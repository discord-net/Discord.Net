/*
 * dave_interfaces.h
 * typedef const char* KeyPairContextType;
 */

global using unsafe KeyPairContextType = byte*;
global using SessionHandle = nuint;
global using CommitResultHandle = nuint;
global using WelcomeResultHandle = nuint;
global using KeyRatchetHandle = nuint;
global using SignaturePrivateKeyHandle = nuint;
global using EncryptorHandle = nuint;
global using DecryptorHandle = nuint;

// typedef void (*DAVEMLSFailureCallback)(const char* source, const char* reason)
global using unsafe MLSFailureCallback = delegate* unmanaged[Cdecl]<
    byte*,
    byte*,
    void*,
    void
>;

// typedef void (*DAVEPairwiseFingerprintCallback)(const uint8_t* fingerprint, size_t length);
global using unsafe PairwiseFingerprintCallback = delegate* unmanaged[Cdecl]<
    byte*,
    nuint,
    void*,
    void
>;

// typedef void (*DAVEEncryptorProtocolVersionChangedCallback)(void);
global using unsafe EncryptorProtocolVersionChangedCallback = delegate* unmanaged[Cdecl]<
    void*,
    void
>;

/*
 * typedef void (*DAVELogSinkCallback)(DAVELoggingSeverity severity,
 *                                     const char* file,
 *                                     int line,
 *                                     const char* message);
 */
global using unsafe LogSinkCallback = delegate* unmanaged[Cdecl]<
    Discord.LibDave.Binding.LoggingSeverity,
    byte*,
    int,
    byte*,
    void
>;

using System.Runtime.InteropServices;

// ReSharper disable UseSymbolAlias

namespace Discord.LibDave.Binding;

/// <summary>
///     An enum representing the different codes within the <see cref="libdave"/> library.
/// </summary>
public enum Codec
{
    /// <summary>
    ///     An unknown codec.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The opus codec.
    /// </summary>
    Opus = 1,

    /// <summary>
    ///     The VP8 codec.
    /// </summary>
    VP8 = 2,

    /// <summary>
    ///     The VP9 codec.
    /// </summary>
    VP9 = 3,

    /// <summary>
    ///     The H264 codec.
    /// </summary>
    H264 = 4,

    /// <summary>
    ///     The H265 codec.
    /// </summary>
    H265 = 5,

    /// <summary>
    ///     The AV1 codec.
    /// </summary>
    AV1 = 6
}

/// <summary>
///     Represents the result of encrypting.
/// </summary>
public enum EncryptorResultCode
{
    /// <summary>
    ///     The encryption was a success.
    /// </summary>
    Success = 0,

    /// <summary>
    ///     The encryption failed.
    /// </summary>
    EncryptionFailure = 1,

    /// <summary>
    ///     No key ratchet available.
    /// </summary>
    MissingKeyRatchet = 2,

    /// <summary>
    ///     Missing cryptographic context.
    /// </summary>
    MissingCryptor = 3,

    /// <summary>
    ///     Too many attempts to encrypt the frame failed.
    /// </summary>
    TooManyAttempts = 4
}

/// <summary>
///     Represents the result of decrypting.
/// </summary>
public enum DecryptorResultCode
{
    /// <summary>
    ///     The decryption was a success.
    /// </summary>
    Success = 0,

    /// <summary>
    ///     The decryption failed.
    /// </summary>
    DecryptionFailure = 1,

    /// <summary>
    ///     The decryption failed because of a missing key ratchet.
    /// </summary>
    MissingKeyRatchet = 2,

    /// <summary>
    ///     The decryption failed because of an invalid nonce.
    /// </summary>
    InvalidNonce = 3,

    /// <summary>
    ///     The decryption failed because of a missing cryptor.
    /// </summary>
    MissingCryptor = 4,
}

/// <summary>
///     Represents the logging severity within the <see cref="libdave"/> library.
/// </summary>
public enum LoggingSeverity
{
    /// <summary>
    ///     A verbose log.
    /// </summary>
    Verbose = 0,

    /// <summary>
    ///     An informative log.
    /// </summary>
    Info = 1,

    /// <summary>
    ///     A warning log.
    /// </summary>
    Warning = 2,

    /// <summary>
    ///     An error log.
    /// </summary>
    Error = 3,

    /// <summary>
    ///     No severity.
    /// </summary>
    None = 4,
}

/// <summary>
///     Represents media type within the <see cref="libdave"/> library.
/// </summary>
public enum MediaType
{
    /// <summary>
    ///     Audio media.
    /// </summary>
    Audio = 0,

    /// <summary>
    ///     Video media.
    /// </summary>
    Video = 1
}

/// <summary>
///     A struct containing statistics related to an encryptor within the <see cref="libdave"/> library.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct EncryptorStats
{
    /// <summary>
    ///     The number of pass through frames.
    /// </summary>
    public readonly ulong PassThroughCount;

    /// <summary>
    ///     The number of successful encryptions.
    /// </summary>
    public readonly ulong EncryptSuccessCount;

    /// <summary>
    ///     The number of failed encryptions.
    /// </summary>
    public readonly ulong EncryptFailureCount;

    /// <summary>
    ///     The duration of encryption.
    /// </summary>
    public readonly ulong EncryptDuration;

    /// <summary>
    ///     The number of encryption attempts.
    /// </summary>
    public readonly ulong EncryptAttempts;

    /// <summary>
    ///     The max number of encryption attempts.
    /// </summary>
    public readonly ulong EncryptMaxAttempts;

    /// <summary>
    ///     The number of missing keys.
    /// </summary>
    public readonly ulong EncryptMissingKeyCount;
}

/// <summary>
///     A struct containing statistics related to a decryptor within the <see cref="libdave"/> library.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct DecryptorStats
{
    /// <summary>
    ///     The number of pass through frames.
    /// </summary>
    public readonly ulong PassThroughCount;

    /// <summary>
    ///     The number of successful decryptions.
    /// </summary>
    public readonly ulong DecryptSuccessCount;

    /// <summary>
    ///     The number of failed decryptions.
    /// </summary>
    public readonly ulong DecryptFailureCount;

    /// <summary>
    ///     The duration of decryption.
    /// </summary>
    public readonly ulong DecryptDuration;

    /// <summary>
    ///     The number of decryption attempts.
    /// </summary>
    public readonly ulong DecryptAttempts;

    /// <summary>
    ///     The number failed decryptions due to missing keys.
    /// </summary>
    public readonly ulong DecryptMissingKeyCount;

    /// <summary>
    ///     The number of failed decryptions due to invalid nonces
    /// </summary>
    public readonly ulong DecryptInvalidNonceCount;
}

/// <summary>
///     A class providing the raw interop with the <c>libdave</c> binding.
/// </summary>
/// <remarks>
///     It's recommended to use <see cref="Dave"/> to interact with <c>libdave</c> in a safe mannar, only use this
///     class if you know what you're doing!!!
/// </remarks>
public static unsafe partial class libdave
{
    /// <summary>
    ///     The name of the <see cref="libdave"/> library file.
    /// </summary>
    public const string LIBRARY_NAME = "libdave";

    // uint16_t daveMaxSupportedProtocolVersion(void)
    /// <summary>
    ///     Gets the maximum protocol version supported by this library.
    /// </summary>
    /// <returns>Maximum supported protocol version number.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveMaxSupportedProtocolVersion")]
    public static partial ushort MaxSupportedProtocolVersion();

    /* DAVESessionHandle daveSessionCreate(
     *     void* context,
     *     const char* authSessionId,
     *     DAVEMLSFailureCallback callback,
     *     void* userData
     * )
     */
    /// <summary>
    ///     Creates a new DAVE session.
    /// </summary>
    /// <param name="context">Currently unused platform-specific context pointer (can be NULL).</param>
    /// <param name="authSessionId">String used to manage persistent key lifetimes (can be NULL).</param>
    /// <param name="callback">Callback invoked on MLS failures.</param>
    /// <param name="userData">User data pointer passed to the callback.</param>
    /// <returns>New session handle, or NULL on failure. Must be destroyed with <see cref="SessionDestroy"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionCreate")]
    public static partial SessionHandle SessionCreate(
        KeyPairContextType context,
        byte* authSessionId,
        MLSFailureCallback callback,
        void* userData
    );

    // void daveSessionDestroy(DAVESessionHandle session)
    /// <summary>
    ///     Destroys a session and frees associated resources.
    /// </summary>
    /// <param name="session">Session handle to destroy.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionDestroy")]
    public static partial void SessionDestroy(
        SessionHandle session
    );

    /*
     * void daveSessionInit(
     *     DAVESessionHandle session,
     *     uint16_t version,
     *     uint64_t groupId,
     *     const char* selfUserId
     * )
     */
    /// <summary>
    ///     Initializes a session with protocol version and group information.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="version">The protocol version to use.</param>
    /// <param name="groupId">The group identifier common to all users in the group.</param>
    /// <param name="selfUserId">The user ID of the local user.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionInit")]
    public static partial void SessionInit(
        SessionHandle session,
        ushort version,
        ulong groupId,
        ReadOnlySpan<byte> selfUserId
    );

    // void daveSessionReset(DAVESessionHandle session)
    /// <summary>
    ///     Resets the session state.
    /// </summary>
    /// <param name="session">The session handle.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionReset")]
    public static partial void SessionReset(SessionHandle session);

    // void daveSessionSetProtocolVersion(DAVESessionHandle session, uint16_t version)
    /// <summary>
    ///     Sets the protocol version for the session.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="version">The protocol version to set.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionSetProtocolVersion")]
    public static partial void SessionSetProtocolVersion(SessionHandle session, ushort version);

    // uint16_t daveSessionGetProtocolVersion(DAVESessionHandle session)
    /// <summary>
    ///     Gets the current protocol version of the session.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <returns>The current protocol version.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetProtocolVersion")]
    public static partial ushort SessionGetProtocolVersion(SessionHandle session);

    /*
     * void daveSessionGetLastEpochAuthenticator(
     *     DAVESessionHandle session,
     *     uint8_t** authenticator,
     *     size_t* length
     * )
     */
    /// <summary>
    ///     Retrieves the authenticator from the last MLS epoch.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="authenticator">The output pointer to authenticator bytes (caller must free</param>
    /// <param name="length">The output pointer to authenticator length</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetLastEpochAuthenticator")]
    public static partial void SessionGetLastEpochAuthenticator(
        SessionHandle session,
        byte** authenticator,
        nint* length
    );

    /*
     * void daveSessionSetExternalSender(
     *     DAVESessionHandle session,
     *     const uint8_t* externalSender,
     *     size_t length
     * )
     */
    /// <summary>
    ///     Sets the external sender credentials for the session.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="externalSender">The external sender credential bytes.</param>
    /// <param name="length">The length of external sender data.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionSetExternalSender")]
    public static partial void SessionSetExternalSender(
        SessionHandle session,
        byte* externalSender,
        nint length
    );

    /*
     * void daveSessionProcessProposals(
     *     DAVESessionHandle session,
     *     const uint8_t* proposals,
     *     size_t length,
     *     const char** recognizedUserIds,
     *     size_t recognizedUserIdsLength,
     *     uint8_t** commitWelcomeBytes,
     *     size_t* commitWelcomeBytesLength
     * )
     */
    /// <summary>
    ///     Processes MLS proposals and generates commit/welcome messages.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="proposals">The serialized proposal bytes.</param>
    /// <param name="length">The length of proposals.</param>
    /// <param name="recognizedUserIds">The array of recognized user ID strings.</param>
    /// <param name="recognizedUserIdsLength">The number of recognized user IDs.</param>
    /// <param name="commitWelcomeBytes">The output buffer to commit/welcome message bytes (caller must free).</param>
    /// <param name="commitWelcomeBytesLength">The output length of the commit/welcome message.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionProcessProposals")]
    public static partial void SessionProcessProposals(
        SessionHandle session,
        byte* proposals,
        nint length,
        byte** recognizedUserIds,
        nint recognizedUserIdsLength,
        byte** commitWelcomeBytes,
        nint* commitWelcomeBytesLength
    );

    /*
     * DAVECommitResultHandle daveSessionProcessCommit(
     *     DAVESessionHandle session,
     *     const uint8_t* commit,
     *     size_t length
     * )
     */
    /// <summary>
    ///     Processes an incoming MLS commit message.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="commit">The serialized commit message bytes.</param>
    /// <param name="length">The length of the commit message.</param>
    /// <returns>The commit result handle. Must be destroyed with <see cref="CommitResultDestroy"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionProcessCommit")]
    public static partial CommitResultHandle SessionProcessCommit(
        SessionHandle session,
        byte* commit,
        nint length
    );

    /*
     * DAVEWelcomeResultHandle daveSessionProcessWelcome(
     *     DAVESessionHandle session,
     *     const uint8_t* welcome,
     *     size_t length,
     *     const char** recognizedUserIds,
     *     size_t recognizedUserIdsLength
     * )
     */
    /// <summary>
    ///     Processes an incoming MLS welcome message to join a group.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="welcome">Serialized welcome message bytes.</param>
    /// <param name="length">The length of the welcome message.</param>
    /// <param name="recognizedUserIds">Array of recognized user ID strings.</param>
    /// <param name="recognizedUserIdsLength">The number of recognized user IDs.</param>
    /// <returns>The welcome result handle. Must be destroyed with <see cref="WelcomeResultDestroy"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionProcessWelcome")]
    public static partial WelcomeResultHandle SessionProcessWelcome(
        SessionHandle session,
        byte* welcome,
        nint length,
        byte** recognizedUserIds,
        nuint recognizedUserIdsLength
    );

    /*
     * void daveSessionGetMarshalledKeyPackage(
     *     DAVESessionHandle session,
     *     uint8_t** keyPackage,
     *     size_t* length
     * )
     */
    /// <summary>
    ///      Gets the marshalled MLS key package for this session.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="keyPackage">The output buffer to key package bytes (caller must free).</param>
    /// <param name="length">The output length of the key package.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetMarshalledKeyPackage")]
    public static partial void SessionGetMarshalledKeyPackage(
        SessionHandle session,
        byte** keyPackage,
        nint* length
    );

    /*
     * DAVEKeyRatchetHandle daveSessionGetKeyRatchet(
     *     DAVESessionHandle session,
     *     const char* userId
     * )
     */
    /// <summary>
    ///     Gets a key ratchet for a specific user in the session.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="userId">The user ID to get key ratchet for.</param>
    /// <returns>The key ratchet handle. Must be destroyed with <see cref="KeyRatchetDestroy"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetKeyRatchet")]
    public static partial KeyRatchetHandle SessionGetKeyRatchet(
        SessionHandle session,
        byte* userId
    );

    /*
     * void daveSessionGetPairwiseFingerprint(
     *     DAVESessionHandle session,
     *     uint16_t version,
     *     const char* userId,
     *     DAVEPairwiseFingerprintCallback callback,
     *     void* userData
     * )
     */
    /// <summary>
    ///     Computes a pairwise fingerprint for identity verification with another user.
    /// </summary>
    /// <param name="session">The session handle.</param>
    /// <param name="version">The protocol version currently in use.</param>
    /// <param name="userId">The user ID of the remote user to compute the fingerprint for.</param>
    /// <param name="callback">A callback to receive the fingerprint.</param>
    /// <param name="userData">User data passed to callback.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetPairwiseFingerprint")]
    public static partial void SessionGetPairwiseFingerprint(
        SessionHandle session,
        ushort version,
        byte* userId,
        PairwiseFingerprintCallback callback,
        void* userData
    );

    // void daveKeyRatchetDestroy(DAVEKeyRatchetHandle keyRatchet)
    /// <summary>
    ///     Destroys a key ratchet and frees associated resources.
    /// </summary>
    /// <param name="keyRatchet">The key ratchet handle to destroy.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveKeyRatchetDestroy")]
    public static partial void KeyRatchetDestroy(
        KeyRatchetHandle keyRatchet
    );

    // bool daveCommitResultIsFailed(DAVECommitResultHandle commitResultHandle)
    /// <summary>
    ///     Checks if processing the commit failed.
    /// </summary>
    /// <param name="commitResultHandle">The commit result handle.</param>
    /// <returns><see langword="true"/> if the commit processing failed; otherwise <see langword="false"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultIsFailed")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CommitResultIsFailed(
        CommitResultHandle commitResultHandle
    );

    // bool daveCommitResultIsIgnored(DAVECommitResultHandle commitResultHandle)
    /// <summary>
    ///     Checks if the commit should be ignored.
    /// </summary>
    /// <param name="commitResultHandle">The commit result handle.</param>
    /// <returns><see langword="true"/> if the commit should be ignored; otherwise <see langword="false"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultIsIgnored")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CommitResultIsIgnored(
        CommitResultHandle commitResultHandle
    );

    /*
     * void daveCommitResultGetRosterMemberIds(
     *     DAVECommitResultHandle commitResultHandle,
     *     uint64_t** rosterIds,
     *     size_t* rosterIdsLength
     * )
     */
    /// <summary>
    ///      Gets the list of member IDs in the roster after the commit.
    /// </summary>
    /// <param name="commitResultHandle">The commit result handle.</param>
    /// <param name="rosterIds">The output buffer to array of roster member IDs (caller must free).</param>
    /// <param name="rosterIdsLength">The output length of the roster member IDs array.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultGetRosterMemberIds")]
    public static partial void CommitResultGetRosterMemberIds(
        CommitResultHandle commitResultHandle,
        ulong** rosterIds,
        nuint* rosterIdsLength
    );

    /*
     * void daveCommitResultGetRosterMemberSignature(
     *     DAVECommitResultHandle commitResultHandle,
     *     uint64_t rosterId,
     *     uint8_t** signature,
     *     size_t* signatureLength
     * )
     */
    /// <summary>
    ///     Gets the signature for a specific roster member.
    /// </summary>
    /// <param name="commitResultHandle">The commit result handle.</param>
    /// <param name="rosterId">The roster member ID.</param>
    /// <param name="signature">The output buffer to signature bytes (caller must free).</param>
    /// <param name="signatureLength">The output length of the signature.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultGetRosterMemberSignature")]
    public static partial void CommitResultGetRosterMemberSignature(
        CommitResultHandle commitResultHandle,
        ulong rosterId,
        byte** signature,
        nint* signatureLength
    );

    //  void daveCommitResultDestroy(DAVECommitResultHandle commitResultHandle)
    /// <summary>
    ///     Destroys a commit result and frees associated resources.
    /// </summary>
    /// <param name="commitResultHandle">The commit result handle to destroy.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultDestroy")]
    public static partial void CommitResultDestroy(
        CommitResultHandle commitResultHandle
    );

    /*
     * void daveWelcomeResultGetRosterMemberIds(
     *     DAVEWelcomeResultHandle welcomeResultHandle,
     *     uint64_t** rosterIds,
     *     size_t* rosterIdsLength
     * )
     */
    /// <summary>
    ///     Gets the list of member IDs in the roster from the welcome message.
    /// </summary>
    /// <param name="welcomeResultHandle">The welcome result handle.</param>
    /// <param name="rosterIds">The output buffer to array of roster member IDs (caller must free).</param>
    /// <param name="rosterIdsLength">The output length of the roster member IDs array.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveWelcomeResultGetRosterMemberIds")]
    public static partial void WelcomeResultGetRosterMemberIds(
        WelcomeResultHandle welcomeResultHandle,
        ulong** rosterIds,
        nint* rosterIdsLength
    );

    /*
     * void daveWelcomeResultGetRosterMemberSignature(
     *     DAVEWelcomeResultHandle welcomeResultHandle,
     *     uint64_t rosterId,
     *     uint8_t** signature,
     *     size_t* signatureLength
     * )
     */
    /// <summary>
    ///     Gets the signature for a specific roster member.
    /// </summary>
    /// <param name="welcomeResultHandle">The welcome result handle.</param>
    /// <param name="rosterId">The roster member ID.</param>
    /// <param name="signature">The output buffer to signature bytes (caller must free).</param>
    /// <param name="signatureLength">The output length of the signature.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveWelcomeResultGetRosterMemberSignature")]
    public static partial void WelcomeResultGetRosterMemberSignature(
        WelcomeResultHandle welcomeResultHandle,
        ulong rosterId,
        byte** signature,
        nint* signatureLength
    );

    // void daveWelcomeResultDestroy(DAVEWelcomeResultHandle welcomeResultHandle)
    /// <summary>
    ///     Destroys a welcome result and frees associated resources.
    /// </summary>
    /// <param name="welcomeResultHandle">The welcome result handle to destroy.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveWelcomeResultDestroy")]
    public static partial void WelcomeResultDestroy(
        WelcomeResultHandle welcomeResultHandle
    );

    // DAVEEncryptorHandle daveEncryptorCreate(void)
    /// <summary>
    ///     Creates a new media frame encryptor.
    /// </summary>
    /// <returns>The wew encryptor handle. Must be destroyed with <see cref="EncryptorDestroy"/>.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorCreate")]
    public static partial EncryptorHandle EncryptorCreate();

    // void daveEncryptorDestroy(DAVEEncryptorHandle encryptor)
    /// <summary>
    ///     Destroys an encryptor and frees associated resources.
    /// </summary>
    /// <param name="encryptor">The encryptor handle to destroy.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorDestroy")]
    public static partial void EncryptorDestroy(
        EncryptorHandle encryptor
    );

    /*
     * void daveEncryptorSetKeyRatchet(
     *     DAVEEncryptorHandle encryptor,
     *     DAVEKeyRatchetHandle keyRatchet
     * )
     */
    /// <summary>
    ///     Sets the key ratchet for encryption.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="keyRatchet">The key ratchet to use for encryption (does <b>not</b> take ownership).</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorSetKeyRatchet")]
    public static partial void EncryptorSetKeyRatchet(
        EncryptorHandle encryptor,
        KeyRatchetHandle keyRatchet
    );

    /*
     * void daveEncryptorSetPassthroughMode(
     *     DAVEEncryptorHandle encryptor,
     *     bool passthroughMode
     * )
     */
    /// <summary>
    ///     Enables or disables passthrough mode (frames pass through unencrypted).
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="passthroughMode">
    ///     <see langword="true"/> to enable passthrough; <see langword="false"/> to encrypt.
    /// </param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorSetPassthroughMode")]
    public static partial void EncryptorSetPassthroughMode(
        EncryptorHandle encryptor,
        [MarshalAs(UnmanagedType.Bool)] bool passthroughMode
    );

    /*
     * void daveEncryptorAssignSsrcToCodec(
     *     DAVEEncryptorHandle encryptor,
     *     uint32_t ssrc,
     *     DAVECodec codecType
     * )
     */
    /// <summary>
    ///     Associates an SSRC (Synchronization Source) with a specific codec.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="ssrc">The SSRC identifier.</param>
    /// <param name="codecType">The codec type for this SSRC.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorAssignSsrcToCodec")]
    public static partial void EncryptorAssignSsrcToCodec(
        EncryptorHandle encryptor,
        uint ssrc,
        Codec codecType
    );

    // uint16_t daveEncryptorGetProtocolVersion(DAVEEncryptorHandle encryptor)
    /// <summary>
    ///     Gets the current protocol version used by the encryptor.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <returns>The protocol version number.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorGetProtocolVersion")]
    public static partial ushort EncryptorGetProtocolVersion(
        EncryptorHandle encryptor
    );

    /*
     * size_t daveEncryptorGetMaxCiphertextByteSize(
     *     DAVEEncryptorHandle encryptor,
     *     DAVEMediaType mediaType,
     *     size_t frameSize
     * )
     */
    /// <summary>
    ///     Calculates the maximum ciphertext size for a given plaintext frame size.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="mediaType">The media type (audio or video).</param>
    /// <param name="frameSize">The size of plaintext frame in bytes.</param>
    /// <returns>The maximum possible ciphertext size in bytes.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorGetMaxCiphertextByteSize")]
    public static partial nint EncryptorGetMaxCiphertextByteSize(
        EncryptorHandle encryptor,
        MediaType mediaType,
        nint frameSize
    );

    // bool daveEncryptorHasKeyRatchet(DAVEEncryptorHandle encryptor);
    /// <summary>
    ///     Checks if the encryptor has a key ratchet.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <returns>
    ///     <see langword="true"/> if the encryptor has the given key ratchet; otherwise <see langword="false"/>.
    /// </returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorHasKeyRatchet")]
    public static partial bool EncryptorHasKeyRatchet(EncryptorHandle encryptor);

    //bool daveEncryptorIsPassthroughMode(DAVEEncryptorHandle encryptor);
    /// <summary>
    ///     Checks if the encryptor is in passthrough mode.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <returns>
    ///     <see langword="true"/> if the encryptor is in passthrough mode; otherwise <see langword="false"/>.
    /// </returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorIsPassthroughMode")]
    public static partial bool EncryptorIsPassthroughMode(EncryptorHandle encryptor);

    /*
     * DAVEEncryptorResultCode daveEncryptorEncrypt(
     *     DAVEEncryptorHandle encryptor,
     *     DAVEMediaType mediaType,
     *     uint32_t ssrc,
     *     const uint8_t* frame,
     *     size_t frameLength,
     *     uint8_t* encryptedFrame,
     *     size_t encryptedFrameCapacity,
     *     size_t* bytesWritten
     * )
     */
    /// <summary>
    ///     Encrypts a media frame.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="mediaType">The media type (audio or video).</param>
    /// <param name="ssrc">The SSRC of the stream.</param>
    /// <param name="frame">The pointer to plaintext frame data.</param>
    /// <param name="frameLength">The length of plaintext frame.</param>
    /// <param name="encryptedFrame">The pointer to the output buffer the encrypted frame will be written to.</param>
    /// <param name="encryptedFrameCapacity">The capacity of the output buffer.</param>
    /// <param name="bytesWritten">The number of bytes written to the output buffer.</param>
    /// <returns>The result code indicating success or failure.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorEncrypt")]
    public static partial EncryptorResultCode EncryptorEncrypt(
        EncryptorHandle encryptor,
        MediaType mediaType,
        uint ssrc,
        byte* frame,
        nint frameLength,
        byte* encryptedFrame,
        nint encryptedFrameCapacity,
        nint* bytesWritten
    );

    /*
     * void daveEncryptorSetProtocolVersionChangedCallback(
     *     DAVEEncryptorHandle encryptor,
     *     DAVEEncryptorProtocolVersionChangedCallback callback,
     *     void* userData
     * )
     */
    /// <summary>
    ///     Sets a callback to be notified when the protocol version changes.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="callback">The callback function.</param>
    /// <param name="userData">User data passed to callback.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorSetProtocolVersionChangedCallback")]
    public static partial void EncryptorSetProtocolVersionChangedCallback(
        EncryptorHandle encryptor,
        EncryptorProtocolVersionChangedCallback callback,
        void* userData
    );

    /*
     * void daveEncryptorGetStats(
     *     DAVEEncryptorHandle encryptor,
     *     DAVEMediaType mediaType,
     *     DAVEEncryptorStats* stats
     * )
     */
    /// <summary>
    ///     Gets encryption statistics.
    /// </summary>
    /// <param name="encryptor">The encryptor handle.</param>
    /// <param name="mediaType">The media type (audio or video).</param>
    /// <param name="stats">Pointer to the stats structure to be filled.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorGetStats")]
    public static partial void EncryptorGetStats(
        EncryptorHandle encryptor,
        MediaType mediaType,
        EncryptorStats* stats
    );

    // DAVEDecryptorHandle daveDecryptorCreate(void)
    /// <summary>
    ///     Creates a new media frame decryptor
    /// </summary>
    /// <returns>
    ///     The new decryptor handle. Must be destroyed with <see cref="DecryptorDestroy"/>.
    /// </returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorCreate")]
    public static partial DecryptorHandle DecryptorCreate();

    // void daveDecryptorDestroy(DAVEDecryptorHandle decryptor)
    /// <summary>
    ///     Destroys a decryptor and frees associated resources.
    /// </summary>
    /// <param name="decryptor">The decryptor handle to destroy.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorDestroy")]
    public static partial void DecryptorDestroy(
        DecryptorHandle decryptor
    );

    /*
     * void daveDecryptorTransitionToKeyRatchet(
     *     DAVEDecryptorHandle decryptor,
     *     DAVEKeyRatchetHandle keyRatchet
     * )
     */
    /// <summary>
    ///     Transitions the decryptor to use a new key ratchet.
    /// </summary>
    /// <param name="decryptor">The decryptor handle.</param>
    /// <param name="keyRatchet">The new key ratchet to transition to (does <b>not</b> take ownership)</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorTransitionToKeyRatchet")]
    public static partial void DecryptorTransitionToKeyRatchet(
        DecryptorHandle decryptor,
        KeyRatchetHandle keyRatchet
    );

    /*
     * void daveDecryptorTransitionToPassthroughMode(
     *     DAVEDecryptorHandle decryptor,
     *     bool passthroughMode
     * )
     */
    /// <summary>
    ///     Transitions to or from passthrough mode.
    /// </summary>
    /// <param name="decryptor">The decryptor handle.</param>
    /// <param name="passthroughMode">
    ///     <see langword="true"/> to enable passthrough; <see langword="false"/> to decrypt.
    /// </param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorTransitionToPassthroughMode")]
    public static partial void DecryptorTransitionToPassthroughMode(
        DecryptorHandle decryptor,
        [MarshalAs(UnmanagedType.Bool)] bool passthroughMode
    );

    /*
     * DAVEDecryptorResultCode daveDecryptorDecrypt(
     *     DAVEDecryptorHandle decryptor,
     *     DAVEMediaType mediaType,
     *     const uint8_t* encryptedFrame,
     *     size_t encryptedFrameLength,
     *     uint8_t* frame,
     *     size_t frameCapacity,
     *     size_t* bytesWritten
     * )
     */
    /// <summary>
    ///     Decrypts an encrypted media frame.
    /// </summary>
    /// <param name="decryptor">The decryptor handle.</param>
    /// <param name="mediaType">The media type (audio or video).</param>
    /// <param name="encryptedFrame">The pointer to the encrypted frame data.</param>
    /// <param name="encryptedFrameLength">The length of the encrypted frame.</param>
    /// <param name="frame">Pointer to the output buffer the decrypted frame will be written to.</param>
    /// <param name="frameCapacity">The capacity of the output buffer.</param>
    /// <param name="bytesWritten">The number of bytes written to the output buffer.</param>
    /// <returns>The result code indicating success or failure.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorDecrypt")]
    public static partial DecryptorResultCode DecryptorDecrypt(
        DecryptorHandle decryptor,
        MediaType mediaType,
        byte* encryptedFrame,
        nint encryptedFrameLength,
        byte* frame,
        nint frameCapacity,
        nint* bytesWritten
    );

    /*
     * size_t daveDecryptorGetMaxPlaintextByteSize(
     *     DAVEDecryptorHandle decryptor,
     *     DAVEMediaType mediaType,
     *     size_t encryptedFrameSize
     * )
     */
    /// <summary>
    ///     Calculates the maximum plaintext size for a given ciphertext frame size.
    /// </summary>
    /// <param name="decryptor">The decryptor handle.</param>
    /// <param name="mediaType">The media type (audio or video).</param>
    /// <param name="encryptedFrameSize">The size of encrypted frame in bytes.</param>
    /// <returns>The maximum possible plaintext size in bytes.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorGetMaxPlaintextByteSize")]
    public static partial nint DecryptorGetMaxPlaintextByteSize(
        DecryptorHandle decryptor,
        MediaType mediaType,
        nint encryptedFrameSize
    );

    /*
     * void daveDecryptorGetStats(
     *     DAVEDecryptorHandle decryptor,
     *     DAVEMediaType mediaType,
     *     DAVEDecryptorStats* stats
     * )
     */
    /// <summary>
    ///     Gets decryption statistics.
    /// </summary>
    /// <param name="decryptor">The decryptor handle.</param>
    /// <param name="mediaType">The media type (audio or video).</param>
    /// <param name="stats">Pointer to the stats structure to be filled</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorGetStats")]
    public static partial void DecryptorGetStats(
        DecryptorHandle decryptor,
        MediaType mediaType,
        DecryptorStats* stats
    );

    // void daveSetLogSinkCallback(DAVELogSinkCallback callback)
    /// <summary>
    ///     Sets a global callback for receiving log messages from the library.
    /// </summary>
    /// <param name="callback">The log sink callback function.</param>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSetLogSinkCallback")]
    public static partial void SetLogSinkCallback(
        LogSinkCallback callback
    );
}
