using System.Runtime;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal static class DependantHandleUtils
{
#if NET8_0_OR_GREATER
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "UnsafeGetTargetAndDependent")]
    public static extern object? UnsafeGetTargetAndDependent(ref DependentHandle handle);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe object? UnsafeGetTargetAndDependent(ref DependentHandle handle)
        => Unsafe.As<IntPtr, object>(ref *(IntPtr*)Unsafe.As<DependentHandle, IntPtr>(ref handle));
#endif
}
