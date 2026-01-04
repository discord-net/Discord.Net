using Discord.LibDave.Binding;
using System;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a decryptor within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the decryptor.</param>
public sealed class DaveDecryptor(DecryptorHandle handle) : IDisposable
{
    /// <summary>
    ///     Gets or sets the ratchet used by this decryptor.
    /// </summary>
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

    /// <summary>
    ///     Prepares this decryptor for transitioning to a new protocol version.
    /// </summary>
    /// <param name="session">The session this decryptor is used by.</param>
    /// <param name="selfUserId">The snowflake identifier of the current user.</param>
    /// <param name="protocolVersion">The protocol version that is being transitioned to.</param>
    public void PrepareTransition(DaveSession session, ulong selfUserId, int? protocolVersion = null)
    {
        protocolVersion ??= session.ProtocolVersion;

        var isDisabled = protocolVersion is Dave.DISABELD_PROTOCOL_VERSION;

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
        => libdave.DecryptorTransitionToPassthroughMode(handle, passthroughMode);

    /// <summary>
    ///     Decrypts a given frame.
    /// </summary>
    /// <param name="encryptedFrame">The encrypted frame to decrypt.</param>
    /// <param name="mediaType">The type of media within the provided encrypted frame.</param>
    /// <param name="frame">The unencrypted frame.</param>
    /// <returns>The result of decrypting.</returns>
    public unsafe DecryptorResultCode Decrypt(
        ReadOnlyMemory<byte> encryptedFrame,
        MediaType mediaType,
        out ManuallyAllocatedHeapSpan<byte> frame
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

    /// <summary>
    ///     Gets the max size in bytes of plaintext, given the encrypted length and media type.
    /// </summary>
    /// <param name="mediaType">The media type of the encrypted data.</param>
    /// <param name="encryptedFrameSize">The length of the encrypted data.</param>
    /// <returns>The max size in bytes of the plaintext.</returns>
    public int GetMaxPlaintextByteSize(
        MediaType mediaType,
        int encryptedFrameSize
    ) => (int)libdave.DecryptorGetMaxPlaintextByteSize(handle, mediaType, encryptedFrameSize);

    /// <summary>
    ///     Gets statistics about this decryptor.
    /// </summary>
    /// <param name="mediaType">The media type of the statistics to get.</param>
    /// <returns>The statistics for the given media type.</returns>
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

    /// <inheritdoc/>
    public void Dispose()
    {
        Ratchet?.Dispose();
        libdave.DecryptorDestroy(handle);
    }
}
