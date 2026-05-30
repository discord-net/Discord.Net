using Discord.API.Voice;
using Discord.LibDave;
using Discord.LibDave.Binding;
using Discord.Logging;
using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Discord.Audio;

internal sealed class DaveSessionManager : IDisposable
{
    public ushort MaxProtocolVersion => Dave.MaxSupportedProtocolVersion;

    public bool IsDisabled => _session.ProtocolVersion is Dave.DisabledProtocolVersion;

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

        /*
         * TODO: ask staff about correct ways of handling empty VCs
         *
         * When the client connects to an empty voice channel, no protocol transition occurs, leaving the encryptor
         * without a ratchet. This causes the encrypt method to fail.
         *
         * By setting passthrough mode to 'true' before doing any protocol operations ensures that we don't try to
         * encrypt anything before we've set up all the cogs.
         */
        Encryptor.IsInPassthroughMode = true;

        _logger = client.Discord.LogManager.CreateLogger($"Dave #{clientId}");

        _session.OnMLSFailure += (source, reason) =>
        {
            try
            {
                _logger.DebugAsync($"MLS Failure: {source} -> {reason}").GetAwaiter().GetResult();
            }
            catch
            {
                // ignored
            }
        };
    }

    public DaveDecryptor GetOrCreateDecryptor(ulong userId)
    {
        var decryptor = _decryptors.GetOrAdd(
            userId,
            _ => Dave.CreateDecryptor()
        );

        /*
         * ensure the decryptor is configured correctly for the sessions state, this deals with updating the passthrough
         * mode and the decryptors key ratchet.
         */
        decryptor.PrepareTransition(_session, userId, _session.ProtocolVersion);

        return decryptor;
    }

    public void AssignSsrc(uint ssrc)
    {
        // TODO: hardcode opus here?
        Encryptor.AssignSsrcToCodec(ssrc, Codec.Opus);
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

        var seq = BinaryPrimitives.ReadUInt16BigEndian(span[..2]);
        var opCode = (VoiceOpCode)span[2];
        var data = message[3..];

        await _logger.DebugAsync($"Received Binary {opCode} | {data.Length} bytes, sequence #{seq}");

        switch (opCode)
        {
            case VoiceOpCode.DaveMLSExternalSender:
                await OnMLSExternalSenderAsync(data);
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
            await HandleDaveProtocolInitAsync(_session.ProtocolVersion);
        }
        else
        {
            await PrepareProtocolTransitionAsync(transitionId, _session.ProtocolVersion);
        }
    }

    private async Task OnMLSExternalSenderAsync(ReadOnlyMemory<byte> payload)
    {
        await _logger.DebugAsync("Handling external sender package");

        _session.SetExternalSender(payload);
    }

    private async ValueTask OnDaveMLSProposalsAsync(ReadOnlyMemory<byte> payload)
    {
        if (_logger.Level is LogSeverity.Debug)
        {
            await _logger.DebugAsync(
                $"Processing MLS proposal; our users: [{string.Join(", ", _decryptors.Keys)}]"
            );
        }

        using var result = _session.ProcessProposals(
            payload,
            _decryptors.Keys
        );

        await _logger.DebugAsync($"Processed dave MLS proposals, has data?: {result.HasData}");

        if (result.HasData)
            await SendMLSCommitWelcomeAsync(result.ToMemory());
    }

    private async ValueTask OnDaveMLSAnnounceCommitTransactionAsync(ushort transitionId, ReadOnlyMemory<byte> payload)
    {
        if (payload.IsEmpty)
        {
            await _logger.DebugAsync(
                $"Payload for commit was empty; transaction id: {transitionId}, getting new key package..."
            );

            await SendMLSInvalidCommitWelcomeAsync(transitionId);
            await HandleDaveProtocolInitAsync(_session.ProtocolVersion);

            return;
        }

        using var commit = _session.ProcessCommit(payload);

        if (commit.IsIgnored)
        {
            var wasRemoved = _preparedTransitions.TryRemove(transitionId, out _);
            await _logger.DebugAsync(
                $"Commit result ignored, transaction id: {transitionId}, was prepared and removed? {wasRemoved}"
            );
            UpdateEncryptorRatchet(_session.ProtocolVersion);
            return;
        }

        if (commit.IsFailed)
        {
            await _logger.DebugAsync(
                $"Commit result failed, transaction id: {transitionId}, getting new key package..."
            );

            await SendMLSInvalidCommitWelcomeAsync(transitionId);
            await HandleDaveProtocolInitAsync(_session.ProtocolVersion);
        }
        else
        {
            await _logger.DebugAsync(
                $"Commit result succeeded, preparing protocol transition for id {transitionId}..."
            );
            await PrepareProtocolTransitionAsync(transitionId, _session.ProtocolVersion);
        }
    }

    public async Task PrepareProtocolTransitionAsync(ushort transitionId, ushort protocolVersion)
    {
        await _logger.DebugAsync(
            $"Preparing to transition to protocol version {protocolVersion} (transition #{transitionId})"
        );

        foreach (var (id, decryptor) in _decryptors)
        {
            if (id == SelfUserId)
                continue;

            decryptor.PrepareTransition(_session, id, protocolVersion);
        }

        UpdateEncryptorRatchet(protocolVersion);

        if (protocolVersion > Dave.DisabledProtocolVersion)
        {
            // Streams created before DAVE was initialized lack the DaveDecryptStream layer.
            // Rebuild them now that keys are ready.
            await _client.RebuildInputStreamsForDaveAsync();
        }

        if (transitionId is Dave.InitTransitionId)
        {
            return;
        }

        _preparedTransitions[transitionId] = protocolVersion;
        await SendDaveProtocolReadyForTransitionAsync(transitionId);
    }

    public async Task HandleDaveProtocolInitAsync(ushort protocolVersion)
    {
        await _logger.DebugAsync($"Init dave protocol session, version {protocolVersion}");

        if (protocolVersion > Dave.DisabledProtocolVersion)
        {
            await HandlePrepareEpochAsync(Dave.MLSNewGroupExpectedEpoch, protocolVersion);
        }
        else
        {
            await PrepareProtocolTransitionAsync(Dave.InitTransitionId, protocolVersion);
            await ExecuteProtocolTransitionAsync(Dave.InitTransitionId);
        }
    }

    public async Task HandlePrepareEpochAsync(ulong epoch, ushort protocolVersion)
    {
        if (epoch is not Dave.MLSNewGroupExpectedEpoch)
            return;

        await _logger.DebugAsync($"Initializing dave session: epoch {epoch}, protocol version {protocolVersion}");

        _session.Initialize(
            protocolVersion,
            _client.ChannelId,
            SelfUserId
        );

        using var keyPackage = _session.GetMarshalledKeyPackage();
        await SendMLSKeyPackageAsync(keyPackage.ToMemory());
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

        if (protocolVersion is Dave.DisabledProtocolVersion)
        {
            _session.Reset();
            Encryptor.IsInPassthroughMode = true;
        }
    }

    private Task SendMLSCommitWelcomeAsync(ReadOnlyMemory<byte> welcomeMessage)
        => _client.ApiClient.SendBinaryAsync(
            VoiceOpCode.DaveMLSCommitWelcome,
            welcomeMessage
        );

    private Task SendMLSInvalidCommitWelcomeAsync(ushort transitionId)
        => _client.ApiClient.SendAsync(
            VoiceOpCode.DaveMLSInvalidCommitWelcome,
            new DaveMLSTransitionParams() { TransitionId = transitionId }
        );

    private Task SendMLSKeyPackageAsync(ReadOnlyMemory<byte> mlsKeyPackage)
        => _client.ApiClient.SendBinaryAsync(
            VoiceOpCode.DaveMLSKeyPackage,
            mlsKeyPackage
        );

    private Task SendDaveProtocolReadyForTransitionAsync(ushort transitionId)
        => _client.ApiClient.SendAsync(
            VoiceOpCode.DaveTransitionReady,
            new DaveMLSTransitionParams() { TransitionId = transitionId }
        );

    private void UpdateEncryptorRatchet(ushort protocolVersion)
    {
        var ratchet = _session.GetKeyRatchet(SelfUserId);

        var isDisabled = protocolVersion is Dave.DisabledProtocolVersion || ratchet.IsNull;

        Encryptor.IsInPassthroughMode = isDisabled;

        if (!isDisabled)
            Encryptor.Ratchet = ratchet;
    }

    public void Dispose()
    {
        foreach (var (_, decryptor) in _decryptors)
        {
            decryptor.Dispose();
        }
    }
}
