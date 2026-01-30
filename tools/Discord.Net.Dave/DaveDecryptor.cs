using Discord.LibDave.Binding;
using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a decryptor within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the decryptor.</param>
public sealed class DaveDecryptor(DecryptorHandle handle) : IDisposable, INativeHandle
{
    /// <inheritdoc/>
    public bool IsAlive { get; private set; } = handle is not 0;

    /// <inheritdoc/>
    public CommitResultHandle UnderlyingHandle { get; } = handle;

    /// <summary>
    ///     Gets or sets the ratchet used by this decryptor.
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

                    libdave.DecryptorTransitionToKeyRatchet(UnderlyingHandle, value.UnderlyingHandle);
                }
            }

            field = value;
        }
    }

    private readonly Lock _lock = new();

    /// <summary>
    ///     Prepares this decryptor for transitioning to a new protocol version.
    /// </summary>
    /// <param name="session">The session this decryptor is used by.</param>
    /// <param name="selfUserId">The snowflake identifier of the current user.</param>
    /// <param name="protocolVersion">The protocol version that is being transitioned to.</param>
    public void PrepareTransition(DaveSession session, ulong selfUserId, int? protocolVersion = null)
    {
        protocolVersion ??= session.ProtocolVersion;

        var isDisabled = protocolVersion is Dave.DisabledProtocolVersion;

        TransitionToPassthroughMode(isDisabled);

        if (!isDisabled)
        {
            Ratchet = session.GetKeyRatchet(selfUserId);
        }
    }

    /// <summary>
    ///     Transitions this decryptor to the provided passthrough mode.
    /// </summary>
    /// <param name="passthroughMode">
    ///     <see langword="true"/> if this decryptor should pass though data undecrypted; <see langword="false"/> to
    ///     decrypt data.
    /// </param>
    public void TransitionToPassthroughMode(bool passthroughMode)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            libdave.DecryptorTransitionToPassthroughMode(UnderlyingHandle, passthroughMode);
        }
    }

    /// <summary>
    ///     Decrypts a given frame.
    /// </summary>
    /// <param name="encryptedFrame">The encrypted frame to decrypt.</param>
    /// <param name="mediaType">The type of media within the provided encrypted frame.</param>
    /// <param name="frame">The unencrypted frame.</param>
    /// <param name="frameSize">The length of the unencrypted frame.</param>
    /// <returns>The result of decrypting.</returns>
    public unsafe DecryptorResultCode Decrypt(
        ReadOnlyMemory<byte> encryptedFrame,
        MediaType mediaType,
        out IMemoryOwner<byte> frame,
        out int frameSize
    )
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            frameSize = GetMaxPlaintextByteSize(mediaType, encryptedFrame.Length);
            frame = MemoryPool<byte>.Shared.Rent(frameSize);

            try
            {
                nint bytesWritten;
                DecryptorResultCode resultCode;

                fixed (byte* encryptedFramePtr = encryptedFrame.Span)
                fixed (byte* framePtr = frame.Memory.Span)
                {
                    resultCode = libdave.DecryptorDecrypt(
                        UnderlyingHandle,
                        mediaType,
                        encryptedFramePtr,
                        encryptedFrame.Length,
                        framePtr,
                        frameSize,
                        &bytesWritten
                    );
                }

                frameSize = (int)bytesWritten;

                return resultCode;
            }
            catch
            {
                frame.Dispose();
                frame = null!;
                frameSize = 0;
                throw;
            }
        }
    }

    /// <summary>
    ///     Gets the max size in bytes of plaintext, given the encrypted length and media type.
    /// </summary>
    /// <param name="mediaType">The media type of the encrypted data.</param>
    /// <param name="encryptedFrameSize">The length of the encrypted data.</param>
    /// <returns>The max size in bytes of the plaintext.</returns>
    public int GetMaxPlaintextByteSize(
        MediaType mediaType,
        int encryptedFrameSize
    )
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            return (int)libdave.DecryptorGetMaxPlaintextByteSize(UnderlyingHandle, mediaType, encryptedFrameSize);
        }
    }

    /// <summary>
    ///     Gets statistics about this decryptor.
    /// </summary>
    /// <param name="mediaType">The media type of the statistics to get.</param>
    /// <returns>The statistics for the given media type.</returns>
    public unsafe DecryptorStats GetStats(
        MediaType mediaType
    )
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            DecryptorStats stats;

            libdave.DecryptorGetStats(
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
            libdave.DecryptorDestroy(UnderlyingHandle);
            IsAlive = false;
        }
    }
}
