using Discord.LibDave.Binding;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     Represents a session within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the session object within the <see cref="libdave"/> library.</param>
public sealed class DaveSession(SessionHandle handle) : IDisposable
{
    /// <summary>
    ///     Gets or sets the protocol version of this session.
    /// </summary>
    public ushort ProtocolVersion
    {
        get => libdave.SessionGetProtocolVersion(handle);
        set => libdave.SessionSetProtocolVersion(handle, value);
    }

    internal void HandleMLSFailure(string? source, string? reason)
    {
        // TODO
    }

    /// <summary>
    ///     Initializes this session.
    /// </summary>
    /// <param name="protocolVersion">The protocol version of the session.</param>
    /// <param name="groupId">The group ID (channel ID) of the session.</param>
    /// <param name="selfUserId">The ID of the current user.</param>
    public void Initialize(ushort protocolVersion, ulong groupId, ulong selfUserId)
    {
        using var _ = Utils.ToCString(selfUserId, out var selfUserIdStr);

        libdave.SessionInit(
            handle,
            protocolVersion,
            groupId,
            selfUserIdStr
        );
    }

    /// <summary>
    ///     Resets the current session.
    /// </summary>
    public void Reset() => libdave.SessionReset(handle);

    /// <summary>
    ///     Gets the last epoch authenticator of this session.
    /// </summary>
    /// <returns>The last epoch authenticator.</returns>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetLastEpochAuthenticator()
    {
        byte* authenticator;
        nint length;

        libdave.SessionGetLastEpochAuthenticator(
            handle,
            &authenticator,
            &length
        );

        return new ManuallyAllocatedHeapSpan<byte>(authenticator, (int)length);
    }

    /// <summary>
    ///     Sets the external sender of this session.
    /// </summary>
    /// <param name="externalSender">The external sender.</param>
    public unsafe void SetExternalSender(
        ReadOnlyMemory<byte> externalSender
    )
    {
        fixed (byte* ptr = externalSender.Span)
        {
            libdave.SessionSetExternalSender(
                handle,
                ptr,
                externalSender.Length
            );
        }
    }

    /// <summary>
    ///     Processes proposals for this session.
    /// </summary>
    /// <param name="proposals">The proposals to process.</param>
    /// <param name="recognizedUserIds">The snowflake identifiers of any recognized users.</param>
    /// <returns>The result of the proposal.</returns>
    public unsafe ManuallyAllocatedHeapSpan<byte> ProcessProposals(
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
                handle,
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

    /// <summary>
    ///     Processes a commit message for this session.
    /// </summary>
    /// <param name="commit">The commit to process.</param>
    /// <returns>The result of processing the commit.</returns>
    public unsafe DaveCommitResult ProcessCommit(
        ReadOnlyMemory<byte> commit
    )
    {
        fixed (byte* commitPtr = commit.Span)
        {
            return new(
                libdave.SessionProcessCommit(
                    handle,
                    commitPtr,
                    commit.Length
                )
            );
        }
    }

    /// <summary>
    ///     Processes a welcome message for this session.
    /// </summary>
    /// <param name="welcome">The welcome message to process.</param>
    /// <param name="recognizedUserIds">The snowflake identifiers of any recognized users.</param>
    /// <returns>The result of processing the welcome message.</returns>
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
                    handle,
                    welcomePtr,
                    welcome.Length,
                    ids.Pointer,
                    (nuint)ids.Length
                )
            );
        }
    }

    /// <summary>
    ///     Gets the sessions marshalled key package.
    /// </summary>
    /// <returns>The marshalled key package.</returns>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetMarshalledKeyPackage()
    {
        byte* ptr;
        nint length;

        libdave.SessionGetMarshalledKeyPackage(
            handle,
            &ptr,
            &length
        );

        return new(ptr, (int)length);
    }

    /// <summary>
    ///     Gets the key ratchet for a user based on their snowflake identifier.
    /// </summary>
    /// <param name="userId">The snowflake identifier of the user.</param>
    /// <returns>The key ratchet for the given user.</returns>
    public unsafe DaveKeyRatchet GetKeyRatchet(ulong userId)
    {
        using var strHandle = Utils.ToCString(userId, out _);

        return new(
            libdave.SessionGetKeyRatchet(
                handle,
                (CChar*)strHandle.Pointer
            )
        );
    }

    /// <summary>
    ///     Gets a users pairwise fingerprint.
    /// </summary>
    /// <param name="userId">The users snowflake identifier.</param>
    /// <param name="protocolVersion">The current protocol version.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task representing the asynchronous operation of getting a users pairwise fingerprint. The result of
    ///     the task is the fingerprint.
    /// </returns>
    public Task<ManuallyAllocatedHeapSpan<byte>> GetPairwiseFingerprintAsync(
        ulong userId,
        ushort? protocolVersion = null,
        CancellationToken token = default
    )
    {
        protocolVersion ??= ProtocolVersion;

        var tcs = new TaskCompletionSource<ManuallyAllocatedHeapSpan<byte>>();

        token.Register(() => tcs.SetCanceled(token));

        using var userIdStr = Utils.ToCString(userId, out _);

        unsafe
        {
            libdave.SessionGetPairwiseFingerprint(
                handle,
                protocolVersion.Value,
                (CChar*)userIdStr.Pointer,
                (PairwiseFingerprintCallback)Marshal.GetFunctionPointerForDelegate(Callback)
            );
        }

        return tcs.Task;

        unsafe void Callback(byte* ptr, nint length)
            => tcs.TrySetResult(new(ptr, (int)length));
    }

    /// <inheritdoc/>
    public void Dispose()
        => libdave.SessionDestroy(handle);
}
