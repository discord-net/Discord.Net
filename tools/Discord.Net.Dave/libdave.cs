

/*
 * dave_interfaces.h
 * typedef const char* KeyPairContextType;
 */
global using unsafe KeyPairContextType = byte*;

global using CChar = byte;

global using SessionHandle = nuint;
global using CommitResultHandle = nuint;
global using WelcomeResultHandle = nuint;
global using KeyRatchetHandle = nuint;
global using SignaturePrivateKeyHandle = nuint;
global using EncryptorHandle = nuint;
global using DecryptorHandle = nuint;

// typedef void (*DAVEMLSFailureCallback)(const char* source, const char* reason)
global using unsafe MLSFailureCallback = delegate* unmanaged[Cdecl]<byte*, byte*, void>;

// typedef void (*DAVEPairwiseFingerprintCallback)(const uint8_t* fingerprint, size_t length);
global using unsafe PairwiseFingerprintCallback = delegate* unmanaged[Cdecl]<byte*, nuint, void>;

// typedef void (*DAVEEncryptorProtocolVersionChangedCallback)(void);
global using unsafe EncryptorProtocolVersionChangedCallback = delegate* unmanaged[Cdecl]<void>;
/*
 * typedef void (*DAVELogSinkCallback)(DAVELoggingSeverity severity,
 *                                     const char* file,
 *                                     int line,
 *                                     const char* message);
 */
global using unsafe LogSinkCallback = delegate* unmanaged[Cdecl]<Discord.LibDave.Binding.LoggingSeverity, byte*, int, byte*, void>;

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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveMaxSupportedProtocolVersion")]
    public static partial ushort MaxSupportedProtocolVersion();

    /* DAVESessionHandle daveSessionCreate(
     *     void* context,
     *     const char* authSessionId,
     *     DAVEMLSFailureCallback callback
     * )
     */
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionCreate")]
    public static partial SessionHandle SessionCreate(
        KeyPairContextType context,
        CChar* authSessionId,
        MLSFailureCallback callback
    );

    // void daveSessionDestroy(DAVESessionHandle session)
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionInit")]
    public static partial void SessionInit(
        SessionHandle session,
        ushort version,
        ulong groupId,
        ReadOnlySpan<CChar> selfUserId
    );

    // void daveSessionReset(DAVESessionHandle session)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionReset")]
    public static partial void SessionReset(SessionHandle session);

    // void daveSessionSetProtocolVersion(DAVESessionHandle session, uint16_t version)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionSetProtocolVersion")]
    public static partial void SessionSetProtocolVersion(SessionHandle session, ushort version);

    // uint16_t daveSessionGetProtocolVersion(DAVESessionHandle session)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetProtocolVersion")]
    public static partial ushort SessionGetProtocolVersion(SessionHandle session);

    /*
     * void daveSessionGetLastEpochAuthenticator(
     *     DAVESessionHandle session,
     *     uint8_t** authenticator,
     *     size_t* length
     * )
     */
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionProcessProposals")]
    public static partial void SessionProcessProposals(
        SessionHandle session,
        byte* proposals,
        nint length,
        CChar** recognizedUserIds,
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetKeyRatchet")]
    public static partial KeyRatchetHandle SessionGetKeyRatchet(
        SessionHandle session,
        CChar* userId
    );

    /*
     * void daveSessionGetPairwiseFingerprint(
     *     DAVESessionHandle session,
     *     uint16_t version,
     *     const char* userId,
     *     DAVEPairwiseFingerprintCallback callback
     * )
     */
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionGetPairwiseFingerprint")]
    public static partial void SessionGetPairwiseFingerprint(
        SessionHandle session,
        ushort version,
        CChar* userId,
        PairwiseFingerprintCallback callback
    );

    // void daveKeyRatchetDestroy(DAVEKeyRatchetHandle keyRatchet)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveKeyRatchetDestroy")]
    public static partial void KeyRatchetDestroy(
        KeyRatchetHandle keyRatchet
    );

    // bool daveCommitResultIsFailed(DAVECommitResultHandle commitResultHandle)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultIsFailed")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CommitResultIsFailed(
        CommitResultHandle commitResultHandle
    );

    // bool daveCommitResultIsIgnored(DAVECommitResultHandle commitResultHandle)
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveCommitResultGetRosterMemberSignature")]
    public static partial void CommitResultGetRosterMemberSignature(
        CommitResultHandle commitResultHandle,
        ulong rosterId,
        byte** signature,
        nint* signatureLength
    );

    //  void daveCommitResultDestroy(DAVECommitResultHandle commitResultHandle)
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveWelcomeResultGetRosterMemberSignature")]
    public static partial void WelcomeResultGetRosterMemberSignature(
        WelcomeResultHandle welcomeResultHandle,
        ulong rosterId,
        byte** signature,
        nint* signatureLength
    );

    // void daveWelcomeResultDestroy(DAVEWelcomeResultHandle welcomeResultHandle)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveWelcomeResultDestroy")]
    public static partial void WelcomeResultDestroy(
        WelcomeResultHandle welcomeResultHandle
    );

    // DAVEEncryptorHandle daveEncryptorCreate(void)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorCreate")]
    public static partial EncryptorHandle EncryptorCreate();

    // void daveEncryptorDestroy(DAVEEncryptorHandle encryptor)
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorAssignSsrcToCodec")]
    public static partial void EncryptorAssignSsrcToCodec(
        EncryptorHandle encryptor,
        uint ssrc,
        Codec codecType
    );

    // uint16_t daveEncryptorGetProtocolVersion(DAVEEncryptorHandle encryptor)
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorGetMaxCiphertextByteSize")]
    public static partial nint EncryptorGetMaxCiphertextByteSize(
        EncryptorHandle encryptor,
        MediaType mediaType,
        nint frameSize
    );

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
     *     DAVEEncryptorProtocolVersionChangedCallback callback
     * )
     */
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorSetProtocolVersionChangedCallback")]
    public static partial void EncryptorSetProtocolVersionChangedCallback(
        EncryptorHandle encryptor,
        EncryptorProtocolVersionChangedCallback callback
    );

    /*
     * void daveEncryptorGetStats(
     *     DAVEEncryptorHandle encryptor,
     *     DAVEMediaType mediaType,
     *     DAVEEncryptorStats* stats
     * )
     */
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveEncryptorGetStats")]
    public static partial void EncryptorGetStats(
        EncryptorHandle encryptor,
        MediaType mediaType,
        EncryptorStats* stats
    );

    // DAVEDecryptorHandle daveDecryptorCreate(void)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorCreate")]
    public static partial DecryptorHandle DecryptorCreate();

    // void daveDecryptorDestroy(DAVEDecryptorHandle decryptor)
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
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveDecryptorGetStats")]
    public static partial void DecryptorGetStats(
        DecryptorHandle decryptor,
        MediaType mediaType,
        DecryptorStats* stats
    );

    // void daveSetLogSinkCallback(DAVELogSinkCallback callback)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSetLogSinkCallback")]
    public static partial void SetLogSinkCallback(
        LogSinkCallback callback
    );
}
