using Discord.LibDave.Binding;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents an encryptor within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the encryptor object within the <see cref="libdave"/> library.</param>
public sealed class DaveEncryptor(EncryptorHandle handle) : INativeHandle
{
    /// <inheritdoc/>
    public bool IsAlive { get; private set; } = handle is not 0;

    /// <inheritdoc/>
    public EncryptorHandle UnderlyingHandle { get; } = handle;

    /// <summary>
    ///     Gets the protocol version of this encryptor.
    /// </summary>
    public ushort ProtocolVersion
    {
        get
        {
            lock (_lock)
            {
                this.ThrowIfNotAlive();

                return libdave.EncryptorGetProtocolVersion(UnderlyingHandle);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the ratchet used by this encryptor.
    /// </summary>
    public DaveKeyRatchet? Ratchet
    {
        get;
        set
        {
            if (field is not null && (value is null || field.UnderlyingHandle != value.UnderlyingHandle))
                field.Dispose();

            if (value is not null)
            {
                lock (_lock)
                {
                    this.ThrowIfNotAlive();

                    libdave.EncryptorSetKeyRatchet(UnderlyingHandle, value.UnderlyingHandle);
                }
            }

            field = value;
        }
    }

    private readonly Lock _lock = new();

    /// <summary>
    ///     Sets the pass through mode of this encryptor.
    /// </summary>
    /// <param name="passthroughMode">
    ///     <see langword="true"/> if this encryptor should pass though data unencrypted; <see langword="false"/> to
    ///     encrypt data.
    /// </param>
    public void SetPassthroughMode(bool passthroughMode)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            libdave.EncryptorSetPassthroughMode(UnderlyingHandle, passthroughMode);
        }
    }

    /// <summary>
    ///     Assigns a SSRC to a codec used by this encryptor.
    /// </summary>
    /// <param name="ssrc">The SSRC to assign.</param>
    /// <param name="codec">The codec to assign the SSRC to.</param>
    public void AssignSsrcToCodec(uint ssrc, Codec codec)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            libdave.EncryptorAssignSsrcToCodec(UnderlyingHandle, ssrc, codec);
        }
    }

    /// <summary>
    ///     Gets the max size in bytes of ciphertext this encryptor will produce, given the media type and frame size.
    /// </summary>
    /// <param name="mediaType">The media type of the frame.</param>
    /// <param name="frameSize">The size of the frame.</param>
    /// <returns>The max size in bytes of the ciphertext.</returns>
    public int GetMaxCiphertextByteSize(MediaType mediaType, int frameSize)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            return (int)libdave.EncryptorGetMaxCiphertextByteSize(UnderlyingHandle, mediaType, frameSize);
        }
    }

    /// <summary>
    ///     Encrypts a given frame.
    /// </summary>
    /// <param name="frame">The frame to encrypt.</param>
    /// <param name="mediaType">The media type of the frame.</param>
    /// <param name="ssrc">The SSRC of the frame.</param>
    /// <param name="encrypted">The encrypted frame.</param>
    /// <returns>The result of encrypting.</returns>
    public unsafe EncryptorResultCode Encrypt(
        ReadOnlyMemory<byte> frame,
        MediaType mediaType,
        uint ssrc,
        out ManuallyAllocatedHeapSpan<byte> encrypted
    )
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            var outLength = GetMaxCiphertextByteSize(mediaType, frame.Length);

            var encryptedFramePtr = (byte*)NativeMemory.Alloc((nuint)outLength);

            try
            {
                nint bytesWritten;
                EncryptorResultCode result;

                fixed (byte* framePtr = frame.Span)
                {
                    result = libdave.EncryptorEncrypt(
                        UnderlyingHandle,
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
            catch
            {
                NativeMemory.Free(encryptedFramePtr);
                throw;
            }
        }
    }

    /// <summary>
    ///     Gets the statistics of this encryptor.
    /// </summary>
    /// <param name="mediaType">The media type of the stats to get.</param>
    /// <returns>The stats of this encryptor.</returns>
    public unsafe EncryptorStats GetStats(MediaType mediaType)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            EncryptorStats stats;

            libdave.EncryptorGetStats(
                UnderlyingHandle,
                mediaType,
                &stats
            );

            return stats;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (!IsAlive) return;

            Ratchet?.Dispose();
            libdave.EncryptorDestroy(UnderlyingHandle);

            IsAlive = false;
        }
    }
}
