using Discord.LibDave.Binding;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A delegate representing a handler for MLS failure events.
/// </summary>
public delegate void MLSFailureEventHandler(string? source, string? reason);

/// <summary>
///     Represents a session within the <see cref="libdave"/> library.
/// </summary>
/// <param name="handle">The underlying handle to the session object within the <see cref="libdave"/> library.</param>
public sealed class DaveSession(SessionHandle handle) : INativeHandle
{
    /// <inheritdoc/>
    public SessionHandle UnderlyingHandle { get; } = handle;

    /// <inheritdoc/>
    public bool IsAlive { get; private set; } = handle is not 0;

    /// <summary>
    ///     Gets or sets the event handler for MLS failures.
    /// </summary>
    public event MLSFailureEventHandler? OnMLSFailure;

    /// <summary>
    ///     Gets or sets the protocol version of this session.
    /// </summary>
    public ushort ProtocolVersion
    {
        get
        {
            lock (_lock)
            {
                this.ThrowIfNotAlive();

                return libdave.SessionGetProtocolVersion(UnderlyingHandle);
            }
        }
        set
        {
            lock (_lock)
            {
                this.ThrowIfNotAlive();

                libdave.SessionSetProtocolVersion(UnderlyingHandle, value);
            }
        }
    }

    // used to keep the underlying delegate passed to libdave alive
    internal Delegate? MLSFailureCallbackDelegate;

    private readonly Lock _lock = new();

    internal void HandleMLSFailure(string? source, string? reason)
    {
        OnMLSFailure?.Invoke(source, reason);
    }

    /// <summary>
    ///     Initializes this session.
    /// </summary>
    /// <param name="protocolVersion">The protocol version of the session.</param>
    /// <param name="groupId">The group ID (channel ID) of the session.</param>
    /// <param name="selfUserId">The ID of the current user.</param>
    public void Initialize(ushort protocolVersion, ulong groupId, ulong selfUserId)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            using var _ = Utils.ToCString(selfUserId, out var selfUserIdStr);

            libdave.SessionInit(
                UnderlyingHandle,
                protocolVersion,
                groupId,
                selfUserIdStr
            );
        }
    }

    /// <summary>
    ///     Resets the current session.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            libdave.SessionReset(UnderlyingHandle);
        }
    }

    /// <summary>
    ///     Gets the last epoch authenticator of this session.
    /// </summary>
    /// <returns>The last epoch authenticator.</returns>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetLastEpochAuthenticator()
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            byte* authenticator;
            nint length;

            libdave.SessionGetLastEpochAuthenticator(
                UnderlyingHandle,
                &authenticator,
                &length
            );

            return new ManuallyAllocatedHeapSpan<byte>(authenticator, (int)length);
        }
    }

    /// <summary>
    ///     Sets the external sender of this session.
    /// </summary>
    /// <param name="externalSender">The external sender.</param>
    public unsafe void SetExternalSender(
        ReadOnlyMemory<byte> externalSender
    )
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            fixed (byte* ptr = externalSender.Span)
            {
                libdave.SessionSetExternalSender(
                    UnderlyingHandle,
                    ptr,
                    externalSender.Length
                );
            }
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
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            using var ids = Utils.GetIds(recognizedUserIds);

            byte* welcomePtr;
            nint welcomeLength;
            fixed (byte* proposalsPtr = proposals.Span)
            {
                libdave.SessionProcessProposals(
                    UnderlyingHandle,
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
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            fixed (byte* commitPtr = commit.Span)
            {
                return new(
                    libdave.SessionProcessCommit(
                        UnderlyingHandle,
                        commitPtr,
                        commit.Length
                    )
                );
            }
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
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            using var ids = Utils.GetIds(recognizedUserIds);

            fixed (byte* welcomePtr = welcome.Span)
            {
                return new(
                    libdave.SessionProcessWelcome(
                        UnderlyingHandle,
                        welcomePtr,
                        welcome.Length,
                        ids.Pointer,
                        (nuint)ids.Length
                    )
                );
            }
        }
    }

    /// <summary>
    ///     Gets the sessions marshalled key package.
    /// </summary>
    /// <returns>The marshalled key package.</returns>
    public unsafe ManuallyAllocatedHeapSpan<byte> GetMarshalledKeyPackage()
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            byte* ptr;
            nint length;

            libdave.SessionGetMarshalledKeyPackage(
                UnderlyingHandle,
                &ptr,
                &length
            );

            return new(ptr, (int)length);
        }
    }

    /// <summary>
    ///     Gets the key ratchet for a user based on their snowflake identifier.
    /// </summary>
    /// <param name="userId">The snowflake identifier of the user.</param>
    /// <returns>The key ratchet for the given user.</returns>
    public unsafe DaveKeyRatchet GetKeyRatchet(ulong userId)
    {
        lock (_lock)
        {
            this.ThrowIfNotAlive();

            using var strHandle = Utils.ToCString(userId, out _);

            return new(
                libdave.SessionGetKeyRatchet(
                    UnderlyingHandle,
                    (byte*)strHandle.Pointer
                )
            );
        }
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
    public async Task<ManuallyAllocatedHeapSpan<byte>> GetPairwiseFingerprintAsync(
        ulong userId,
        ushort? protocolVersion = null,
        CancellationToken token = default
    )
    {
        LockUtils.Enter(_lock);

        try
        {
            this.ThrowIfNotAlive();

            protocolVersion ??= ProtocolVersion;

            var tcs = new TaskCompletionSource<ManuallyAllocatedHeapSpan<byte>>();
            token.Register(() => tcs.SetCanceled(token));

            Delegate callback;

            using (var userIdStr = Utils.ToCString(userId, out _))
            {
                unsafe
                {
                    callback = Callback;

                    libdave.SessionGetPairwiseFingerprint(
                        UnderlyingHandle,
                        protocolVersion.Value,
                        (byte*)userIdStr.Pointer,
                        (PairwiseFingerprintCallback)Marshal.GetFunctionPointerForDelegate(callback)
                    );
                }
            }

            var result = await tcs.Task;

            GC.KeepAlive(callback);

            return result;

            unsafe void Callback(byte* ptr, nint length)
                => tcs.TrySetResult(new(ptr, (int)length));
        }
        finally
        {
            LockUtils.Exit(_lock);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (!IsAlive) return;

            libdave.SessionDestroy(UnderlyingHandle);
            IsAlive = true;
        }
    }
}
