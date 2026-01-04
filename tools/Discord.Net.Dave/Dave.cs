using Discord.LibDave.Binding;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

/// <summary>
///     A delegate for handling logs from the <see cref="libdave"/> library.
/// </summary>
/// <param name="severity">The log severity.</param>
/// <param name="file">The file that produced the log.</param>
/// <param name="line">The line number of the log.</param>
/// <param name="message">The log message.</param>
public delegate void DaveLogSinkDelegate(
    LoggingSeverity severity,
    string file,
    int line,
    string message
);

/// <summary>
///     A class providing safe interop with the <see cref="libdave"/> library.
/// </summary>
public static class Dave
{
    /// <summary>
    ///     The initial transition id.
    /// </summary>
    public const int INIT_TRANSITION_ID = 0;

    /// <summary>
    ///     The protocol version representing dave e2ee being disabled.
    /// </summary>
    public const int DISABELD_PROTOCOL_VERSION = 0;

    /// <summary>
    ///     The expected epoch of a new MLS group.
    /// </summary>
    public const ulong MLS_NEW_GROUP_EXPECTED_EPOCH = 1;

    /// <summary>
    ///     Gets the max supported protocol version of the <see cref="libdave"/> library.
    /// </summary>
    public static ushort MaxSupportedProtocolVersion => libdave.MaxSupportedProtocolVersion();

    /// <summary>
    ///     Sets the global log sink for the <see cref="libdave"/> binding.
    /// </summary>
    /// <param name="logSink">The delegate to handle logs.</param>
    public static unsafe void SetLogSink(DaveLogSinkDelegate logSink)
    {
        libdave.SetLogSinkCallback(
            (LogSinkCallback)Marshal.GetFunctionPointerForDelegate(WrapperSink)
        );


        void WrapperSink(
            LoggingSeverity severity,
            byte* filePtr,
            int line,
            byte* messagePtr
        )
        {
            var file = Marshal.PtrToStringAnsi((IntPtr)filePtr) ?? string.Empty;
            var message = Marshal.PtrToStringAnsi((IntPtr)messagePtr) ?? string.Empty;

            logSink(severity, file, line, message);
        }
    }

    /// <summary>
    ///     Creates a new <see cref="libdave"/> session.
    /// </summary>
    /// <param name="context">The context for the session.</param>
    /// <param name="authSessionId">The authentication session id.</param>
    /// <returns>
    ///     A <see cref="DaveSession"/> providing safe interop with the <c>libdave</c> library.
    /// </returns>
    public static unsafe DaveSession CreateSession(
        string? context = null,
        string? authSessionId = null
    )
    {
        var contextPtr = Marshal.StringToCoTaskMemAuto(context);
        var authSessionIdPtr =Marshal.StringToCoTaskMemAuto(authSessionId);

        DaveSession? session = null;

        session = new(
            libdave.SessionCreate(
                (byte*) contextPtr,
                (byte*) authSessionIdPtr,
                (MLSFailureCallback)Marshal.GetFunctionPointerForDelegate(OnMLSFailure)
            )
        );

        Marshal.FreeCoTaskMem(contextPtr);
        Marshal.FreeCoTaskMem(authSessionIdPtr);

        return session;

        void OnMLSFailure(byte* sourcePtr, byte* reasonPtr)
        {
            if (session is null) return;

            var source = Marshal.PtrToStringAuto((IntPtr)sourcePtr);
            var reason = Marshal.PtrToStringAuto((IntPtr)reasonPtr);

            session.HandleMLSFailure(source, reason);
        }
    }

    /// <summary>
    ///     Creates a new decryptor.
    /// </summary>
    /// <returns>
    ///     A <see cref="DaveDecryptor"/> providing safe interop with the <see cref="libdave"/> library.
    /// </returns>
    public static DaveDecryptor CreateDecryptor()
        => new(libdave.DecryptorCreate());

    /// <summary>
    ///     Creates a new encryptor.
    /// </summary>
    /// <returns>
    ///     A <see cref="DaveEncryptor"/> providing safe interop with the <see cref="libdave"/> library.
    /// </returns>
    public static DaveEncryptor CreateEncryptor()
        => new(libdave.EncryptorCreate());
}
