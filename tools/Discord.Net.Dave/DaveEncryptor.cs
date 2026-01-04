using Discord.LibDave.Binding;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

public sealed class DaveEncryptor(EncryptorHandle handle) : IDisposable
{
    public ushort ProtocolVersion => libdave.EncryptorGetProtocolVersion(handle);

    public DaveKeyRatchet? Ratchet
    {
        get;
        set
        {
            if (field is not null && (value is null || field.Handle != value.Handle))
                field.Dispose();

            if (value is not null)
                libdave.EncryptorSetKeyRatchet(handle, value.Handle);

            field = value;
        }
    }

    public void SetPassthroughMode(bool passthroughMode)
        => libdave.EncryptorSetPassthroughMode(handle, passthroughMode);

    public void AssignSsrcToCodec(uint ssrc, Codec codec)
        => libdave.EncryptorAssignSsrcToCodec(handle, ssrc, codec);

    public int GetMaxCiphertextByteSize(MediaType mediaType, int frameSize)
        => (int)libdave.EncryptorGetMaxCiphertextByteSize(handle, mediaType, frameSize);

    public unsafe EncryptorResultCode Encrypt(
        ReadOnlyMemory<byte> frame,
        MediaType mediaType,
        uint ssrc,
        out AllocBuffer<byte> encrypted
    )
    {
        var outLength = GetMaxCiphertextByteSize(mediaType, frame.Length);

        var encryptedFramePtr = (byte*)NativeMemory.Alloc((nuint)outLength);

        nint bytesWritten;
        EncryptorResultCode result;

        fixed (byte* framePtr = frame.Span)
        {
            result = libdave.EncryptorEncrypt(
                handle,
                mediaType,
                ssrc,
                framePtr,
                frame.Length,
                encryptedFramePtr,
                outLength,
                &bytesWritten
            );
        }

        encrypted = new(encryptedFramePtr, (int)bytesWritten);
        return result;
    }

    public unsafe EncryptorStats GetStats(MediaType mediaType)
    {
        EncryptorStats stats;

        libdave.EncryptorGetStats(
            handle,
            mediaType,
            &stats
        );

        return stats;
    }

    public void Dispose()
    {
        Ratchet?.Dispose();
        libdave.EncryptorDestroy(handle);
    }
}
