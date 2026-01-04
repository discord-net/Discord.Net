using Discord.LibDave.Binding;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

public sealed class DaveSession : IDisposable
{
    public ushort Version
    {
        get => libdave.SessionGetProtocolVersion(_handle);
        set => libdave.SessionSetProtocolVersion(_handle, value);
    }

    private readonly SessionHandle _handle;
    private bool _isAlive;

    public DaveSession(SessionHandle handle)
    {
        _isAlive = true;
        _handle = handle;
    }

    internal void HandleMLSFailure(string? source, string? reason)
    {

    }

    public void Init(ushort version, ulong groupId, ulong selfUserId)
    {
        using var _ = Utils.ToCString(selfUserId, out var selfUserIdStr);

        libdave.SessionInit(
            _handle,
            version,
            groupId,
            selfUserIdStr
        );
    }

    public void Reset() => libdave.SessionReset(_handle);

    public unsafe AllocBuffer<byte> GetLastEpochAuthenticator()
    {
        byte* authenticator;
        nint length;

        libdave.SessionGetLastEpochAuthenticator(
            _handle,
            &authenticator,
            &length
        );

        return new AllocBuffer<byte>(authenticator, (int)length);
    }

    public unsafe void SetExternalSender(
        ReadOnlyMemory<byte> externalSender
    )
    {
        fixed (byte* ptr = externalSender.Span)
        {
            libdave.SessionSetExternalSender(
                _handle,
                ptr,
                externalSender.Length
            );
        }
    }

    public unsafe AllocBuffer<byte> ProcessProposals(
        ReadOnlyMemory<byte> proposals,
        ICollection<ulong> recognizedUserIds
    )
    {
        using var ids = Utils.GetIds(recognizedUserIds);

        byte* welcomePtr;
        nint welcomeLength;
        fixed (byte* proposalsPtr = proposals.Span)
        {
            libdave.SessionProcessProposals(
                _handle,
                proposalsPtr,
                proposals.Length,
                ids.Pointer,
                ids.Length,
                &welcomePtr,
                &welcomeLength
            );
        }

        return new(welcomePtr, (int)welcomeLength);
    }

    public unsafe DaveCommitResult ProcessCommit(
        ReadOnlyMemory<byte> commit
    )
    {
        fixed (byte* commitPtr = commit.Span)
        {
            return new(
                libdave.SessionProcessCommit(
                    _handle,
                    commitPtr,
                    commit.Length
                )
            );
        }
    }

    public unsafe DaveWelcomeResult ProcessWelcome(
        ReadOnlyMemory<byte> welcome,
        ICollection<ulong> recognizedUserIds
    )
    {
        using var ids = Utils.GetIds(recognizedUserIds);

        fixed (byte* welcomePtr = welcome.Span)
        {
            return new(
                libdave.SessionProcessWelcome(
                    _handle,
                    welcomePtr,
                    welcome.Length,
                    ids.Pointer,
                    (nuint)ids.Length
                )
            );
        }
    }

    public unsafe AllocBuffer<byte> GetMarshalledKeyPackage()
    {
        byte* ptr;
        nint length;

        libdave.SessionGetMarshalledKeyPackage(
            _handle,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    public unsafe DaveKeyRatchet GetKeyRatchet(ulong userId)
    {
        using var strHandle = Utils.ToCString(userId, out _);

        var handle = libdave.SessionGetKeyRatchet(
            _handle,
            (CChar*)strHandle.Pointer
        );

        return new(handle);
    }

    public Task<AllocBuffer<byte>> GetPairwiseFingerprintAsync(
        ulong userId,
        ushort? version = null,
        CancellationToken token = default
    )
    {
        version ??= Version;

        var tcs = new TaskCompletionSource<AllocBuffer<byte>>();

        token.Register(() => tcs.SetCanceled(token));

        using var userIdStr = Utils.ToCString(userId, out _);

        unsafe
        {
            libdave.SessionGetPairwiseFingerprint(
                _handle,
                version.Value,
                (CChar*)userIdStr.Pointer,
                (PairwiseFingerprintCallback)Marshal.GetFunctionPointerForDelegate(Callback)
            );
        }

        return tcs.Task;

        unsafe void Callback(byte* ptr, nint length)
            => tcs.TrySetResult(new(ptr, (int)length));
    }

    public void Dispose()
    {
        if (_isAlive) libdave.SessionDestroy(_handle);
        _isAlive = false;
    }
}
