using System.Runtime.InteropServices;

// ReSharper disable UseSymbolAlias

namespace Discord.Net.LibdaveBinding;

/*
 * dave_interfaces.h
 * typedef const char* KeyPairContextType;
 */
using KeyPairContextType = ReadOnlySpan<char>;
using SessionHandle = nuint;
using CommitResultHandle = nuint;
using WelcomeResultHandle = nuint;
using KeyRatchetHandle = nuint;
using SignaturePrivateKeyHandle = nuint;
using EncryptorHandle = nuint;
using DecryptorHandle = nuint;

// typedef void (*DAVEMLSFailureCallback)(const char* source, const char* reason)
using unsafe MLSFailureCallback = delegate* unmanaged[Cdecl]<char*, char*, void>;

// typedef void (*DAVEPairwiseFingerprintCallback)(const uint8_t* fingerprint, size_t length);
using unsafe PairwiseFingerprintCallback = delegate* unmanaged[Cdecl]<byte*, nuint, void>;

// typedef void (*DAVEEncryptorProtocolVersionChangedCallback)(void);
using unsafe EncryptorProtocolVersionChangedCallback = delegate* unmanaged[Cdecl]<void>;
/*
 * typedef void (*DAVELogSinkCallback)(DAVELoggingSeverity severity,
 *                                     const char* file,
 *                                     int line,
 *                                     const char* message);
 */
using unsafe LogSinkCallback = delegate* unmanaged[Cdecl]<LoggingSeverity, char*, int, char*, void>;

public enum Codec
{
    Unknown = 0,
    Opus = 1,
    VP8 = 2,
    VP9 = 3,
    H264 = 4,
    H265 = 5,
    AV1 = 6
}

public enum EncryptorResultCode
{
    Success = 0,
    EncryptionFailure = 1,
}

public enum DecryptorResultCode
{
    Success = 0,
    DecryptionFailure = 1,
    MissingKeyRatchet = 2,
    InvalidNonce = 3,
    MissingCryptor = 4,
}

public enum LoggingSeverity
{
    Verbose = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    None = 4,
}

public enum MediaType
{
    Audio = 0,
    Video = 1
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct EncryptorStats
{
    public readonly ulong PassThroughCount;
    public readonly ulong EncryptSuccessCount;
    public readonly ulong EncryptFailureCount;
    public readonly ulong EncryptDuration;
    public readonly ulong EncryptAttempts;
    public readonly ulong EncryptMaxAttempts;
    public readonly ulong EncryptMissingKeyCount;
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct DecryptorStats
{
    public readonly ulong PassThroughCount;
    public readonly ulong DecryptSuccessCount;
    public readonly ulong DecryptFailureCount;
    public readonly ulong DecryptDuration;
    public readonly ulong DecryptAttempts;
    public readonly ulong DecryptMissingKeyCount;
    public readonly ulong DecryptInvalidNonceCount;
}

public static unsafe partial class LibDave
{
    public const string LIBRARY_NAME = "libdave";

    // uint16_t daveMaxSupportedProtocolVersion(void)
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveMaxSupportedProtocolVersion")]
    public static partial ushort MaxSupportedProtocolVersion();

    /*
     * DAVESessionHandle daveSessionCreate(
     *   void* context,
     *   const char* authSessionId,
     *   DAVEMLSFailureCallback callback
     * )
     */
    [LibraryImport(LIBRARY_NAME, EntryPoint = "daveSessionCreate")]
    public static partial SessionHandle SessionCreate(
        KeyPairContextType context,
        char* authSessionId,
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
        char* selfUserId
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
        char** reconizedUserIds,
        nint reconizedUserIdsLength,
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
        byte** reconizedUserIds,
        nuint reconizedUserIdsLength
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
        char* userId
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
    public static partial KeyRatchetHandle SessionGetPairwiseFingerprint(
        SessionHandle session,
        ushort version,
        char* userId,
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
        nint bytesWritten
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
