#if NET9_0_OR_GREATER
global using Lock = System.Threading.Lock;
#else
global using Lock = object;
#endif
using System.Runtime.CompilerServices;

namespace Discord.LibDave;

internal static class LockUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter(Lock lockObj)
    {
#if NET9_0_OR_GREATER
        lockObj.Enter();
#else
        Monitor.Enter(lockObj);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Exit(Lock lockObj)
    {
#if NET9_0_OR_GREATER
        lockObj.Exit();
#else
        Monitor.Exit(lockObj);
#endif
    }
}
