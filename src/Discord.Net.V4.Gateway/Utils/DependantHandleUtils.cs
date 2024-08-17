using System.Runtime;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal static class DependantHandleUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe object? UnsafeGetTargetAndDependent(ref DependentHandle handle)
        => Unsafe.As<IntPtr, object>(ref *(IntPtr*)Unsafe.As<DependentHandle, IntPtr>(ref handle));
}
