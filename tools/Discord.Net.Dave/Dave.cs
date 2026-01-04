using Discord.LibDave.Binding;
using System.Runtime.InteropServices;

namespace Discord.LibDave;

public static class Dave
{
    public const int INIT_TRANSITION_ID = 0;
    public const int DISABELD_PROTOCOL_VERSION = 0;
    public const ulong MLS_NEW_GROUP_EXPECTED_EPOCH = 1;

    public static ushort MaxSupportedProtocolVersion => libdave.MaxSupportedProtocolVersion();

    public static unsafe DaveSession CreateSession(
        string? context = null,
        string? authSessionId = null,
        CancellationToken token = default
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

    public static DaveDecryptor CreateDecryptor()
        => new(libdave.DecryptorCreate());

    public static DaveEncryptor CreateEncryptor()
        => new(libdave.EncryptorCreate());
}
