using Discord.API.Voice;
using Discord.LibDave;
using Discord.LibDave.Binding;
using Discord.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Discord.Audio;

internal sealed class DaveSessionManager : IDisposable
{
    public ushort MaxProtocolVersion => Dave.MaxSupportedProtocolVersion;

    public bool IsDisabled => _session.Version is Dave.DISABELD_PROTOCOL_VERSION;

    public ulong SelfUserId => _client.Discord.CurrentUser.Id;

    public DaveEncryptor Encryptor { get; }

    private readonly DaveSession _session;

    private readonly ConcurrentDictionary<ulong, DaveDecryptor> _decryptors;
    private readonly AudioClient _client;
    private readonly ConcurrentDictionary<ushort, ushort> _preparedTransitions;

    private readonly Logger _logger;

    public DaveSessionManager(AudioClient client, int clientId)
    {
        _client = client;
        _decryptors = [];
        _preparedTransitions = [];
        _session = Dave.CreateSession();

        Encryptor = Dave.CreateEncryptor();

        _logger = client.Discord.LogManager.CreateLogger($"Dave #{client}");
    }

    public DaveDecryptor GetDecryptor(ulong userId)
        => _decryptors.GetOrAdd(
            userId,
            _ => Dave.CreateDecryptor()
        );

    public void AssignSsrc(uint ssrc)
    {
        // TODO: hardcode opus here?
        Encryptor.AssignSsrcToCodec(ssrc, Codec.Opus);
    }

    public void AddUser(ulong userId)
    {
        _decryptors.AddOrUpdate(
            userId,
            _ => Dave.CreateDecryptor(),
            (_, existing) => existing
        );
    }

    public bool RemoveUser(ulong id)
    {
        if (_decryptors.TryRemove(id, out var decryptor))
        {
            decryptor.Dispose();
            return true;
        }

        return false;
    }

    public async Task OnBinaryMessageAsync(ReadOnlyMemory<byte> message)
    {
        var span = message.Span;

        var seq = BitConverter.ToUInt16(span[..2]);
        var opCode = (VoiceOpCode)span[2];
        var data = message[2..];

        switch (opCode)
        {
            case VoiceOpCode.DaveMLSExternalSender:
                OnMLSExternalSender(data);
                break;
            case VoiceOpCode.DaveMLSProposals:
                await OnDaveMLSProposalsAsync(data);
                break;
            case VoiceOpCode.DaveAnnounceCommitTransaction:
                await OnDaveMLSAnnounceCommitTransactionAsync(
                    BitConverter.ToUInt16(data[..2].Span),
                    data[2..]
                );
                break;
            case VoiceOpCode.DaveMLSWelcome:
                await OnMLSWelcomeAsync(
                    BitConverter.ToUInt16(data[..2].Span),
                    data[2..]
                );
                break;
            default:
                await _logger.WarningAsync(
                    $"Unknown op code {opCode}"
                );
                return;
        }
    }

    private async ValueTask OnMLSWelcomeAsync(
        ushort transitionId,
        ReadOnlyMemory<byte> payload
    )
    {
        using var welcomeResult = _session.ProcessWelcome(payload, _decryptors.Keys);

        if (welcomeResult.IsNull)
        {
            await SendMLSInvalidCommitWelcomeAsync(transitionId);
            await HandleDaveProtocolInitAsync(transitionId);
        }
        else
        {
            await PrepareProtocolTransitionAsync(transitionId, _session.Version);

            if (transitionId is not Dave.INIT_TRANSITION_ID)
                await SendDaveProtocolReadyForTransitionAsync(transitionId);
        }
    }

    private void OnMLSExternalSender(ReadOnlyMemory<byte> payload)
        => _session.SetExternalSender(payload);

    private async ValueTask OnDaveMLSProposalsAsync(ReadOnlyMemory<byte> payload)
    {
        using var result = _session.ProcessProposals(
            payload,
            _decryptors.Keys
        );

        if (result.HasData)
        {
            await _client.ApiClient.WebSocketClient.SendAsync(
                result.ToArray(),
                0,
                result.Length,
                isText: false
            );
        }
    }

    private async ValueTask OnDaveMLSAnnounceCommitTransactionAsync(ushort transitionId, ReadOnlyMemory<byte> payload)
    {
        using var commit = _session.ProcessCommit(payload);

        if (commit.IsIgnored)
        {
            _preparedTransitions.TryRemove(transitionId, out _);
            return;
        }

        if (commit.IsFailed)
        {
            await SendMLSInvalidCommitWelcomeAsync(transitionId);
            using var keyPackage = _session.GetMarshalledKeyPackage();
            await SendMLSKeyPackageAsync(keyPackage);

            await HandleDaveProtocolInitAsync(transitionId);
        }
        else
        {
            await PrepareProtocolTransitionAsync(transitionId, _session.Version);

            if (transitionId is not Dave.INIT_TRANSITION_ID)
                await SendDaveProtocolReadyForTransitionAsync(transitionId);
        }
    }

    public async Task PrepareProtocolTransitionAsync(ushort transitionId, ushort protocolVersion)
    {
        await _logger.DebugAsync(
            $"Preparing to transition to protocol version {protocolVersion} (tranisition #{transitionId})"
        );

        foreach (var (id, decryptor) in _decryptors)
        {
            if (id == SelfUserId) continue;

            decryptor.PrepareTransition(_session, SelfUserId, protocolVersion);
        }

        if (transitionId is Dave.INIT_TRANSITION_ID && protocolVersion is not Dave.DISABELD_PROTOCOL_VERSION)
        {
            Encryptor.Ratchet = _session.GetKeyRatchet(SelfUserId);
        }
        else
        {
            _preparedTransitions[transitionId] = protocolVersion;
        }
    }

    public async Task HandleDaveProtocolInitAsync(ushort protocolVersion)
    {
        await _logger.DebugAsync($"Init dave protocol session, version {protocolVersion}");

        if (protocolVersion > Dave.DISABELD_PROTOCOL_VERSION)
        {
            HandlePrepareEpoch(Dave.MLS_NEW_GROUP_EXPECTED_EPOCH, protocolVersion);
            using var keyPackage = _session.GetMarshalledKeyPackage();
            await SendMLSKeyPackageAsync(keyPackage);
        }
        else
        {
            await PrepareProtocolTransitionAsync(Dave.INIT_TRANSITION_ID, protocolVersion);
            await ExecuteProtocolTransitionAsync(Dave.INIT_TRANSITION_ID);
        }
    }

    public void HandlePrepareEpoch(ulong epoch, ushort protocolVersion)
    {
        if (epoch is not Dave.MLS_NEW_GROUP_EXPECTED_EPOCH) return;

        _session.Init(
            protocolVersion,
            _client.ChannelId,
            SelfUserId
        );
    }

    public async Task ExecuteProtocolTransitionAsync(ushort transitionId)
    {
        if (!_preparedTransitions.TryRemove(transitionId, out var protocolVersion))
        {
            await _logger.WarningAsync(
                $"Unexpected transition id: {transitionId}"
            );
            return;
        }

        await _logger.DebugAsync(
            $"Executing tranisition to protocol version {protocolVersion} (transition #{transitionId})"
        );

        if (protocolVersion is Dave.DISABELD_PROTOCOL_VERSION)
        {
            _session.Reset();
            Encryptor.SetPassthroughMode(true);
        }
    }

    private Task SendMLSInvalidCommitWelcomeAsync(ushort transitionId)
        => _client.ApiClient.SendAsync(
            VoiceOpCode.DaveMLSInvalidCommitWelcome,
            new DaveMLSTransitionParams() { TransitionId = transitionId }
        );

    private Task SendMLSKeyPackageAsync(AllocBuffer<byte> mlsKeyPackage)
        => _client.ApiClient.SendBinaryAsync(
            VoiceOpCode.DaveMLSKeyPackage,
            mlsKeyPackage.ToArray()
        );

    private Task SendDaveProtocolReadyForTransitionAsync(ushort transitionId)
        => _client.ApiClient.SendAsync(
            VoiceOpCode.DaveTransitionReady,
            new DaveMLSTransitionParams() { TransitionId = transitionId }
        );

    public void Dispose()
    {
        foreach (var (_, decryptor) in _decryptors)
        {
            decryptor.Dispose();
        }
    }
}
