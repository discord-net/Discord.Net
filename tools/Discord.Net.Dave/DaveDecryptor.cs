using Discord.LibDave.Binding;
using System;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

public sealed class DaveDecryptor(DecryptorHandle handle) : IDisposable
{
    public DaveKeyRatchet? Ratchet
    {
        get;
        set
        {
            if (field is not null && (value is null || field.Handle != value.Handle))
                field.Dispose();

            if (value is not null)
                libdave.DecryptorTransitionToKeyRatchet(handle, value.Handle);

            field = value;
        }
    }

    public void PrepareTransition(DaveSession session, ulong selfUserId, int? protocolVersion = null)
    {
        protocolVersion ??= session.Version;

        var isDisabled = protocolVersion is Dave.DISABELD_PROTOCOL_VERSION;

        TransitionToPassthroughMode(isDisabled);

        if (!isDisabled)
        {
            Ratchet = session.GetKeyRatchet(selfUserId);
        }
    }

    public void TransitionToPassthroughMode(bool passthroughMode)
        => libdave.DecryptorTransitionToPassthroughMode(handle, passthroughMode);

    public unsafe DecryptorResultCode Decrypt(
        ReadOnlyMemory<byte> encryptedFrame,
        MediaType mediaType,
        out AllocBuffer<byte> frame
    )
    {
        var plaintextSize = GetMaxPlaintextByteSize(mediaType, encryptedFrame.Length);
        var framePtr = (byte*)NativeMemory.Alloc((nuint)plaintextSize);

        nint bytesWritten;
        DecryptorResultCode resultCode;

        fixed (byte* encryptedFramePtr = encryptedFrame.Span)
        {
            resultCode = libdave.DecryptorDecrypt(
                handle,
                mediaType,
                encryptedFramePtr,
                encryptedFrame.Length,
                framePtr,
                plaintextSize,
                &bytesWritten
            );
        }

        frame = new(framePtr, (int)bytesWritten);
        return resultCode;
    }

    public int GetMaxPlaintextByteSize(
        MediaType mediaType,
        int encryptedFrameSize
    ) => (int)libdave.DecryptorGetMaxPlaintextByteSize(handle, mediaType, encryptedFrameSize);

    public unsafe DecryptorStats GetStats(
        MediaType mediaType
    )
    {
        DecryptorStats stats;

        libdave.DecryptorGetStats(
            handle,
            mediaType,
            &stats
        );

        return stats;
    }

    public void Dispose()
    {
        Ratchet?.Dispose();
        libdave.DecryptorDestroy(handle);
    }
}
