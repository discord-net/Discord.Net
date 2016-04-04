using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    internal static class TaskHelper
    {
#if NETSTANDARD1_4
        public static Task CompletedTask => Task.CompletedTask;
#else
        public static Task CompletedTask => Task.Delay(0);
#endif

        public static Task CreateLongRunning(Action action, CancellationToken cancelToken)
            => Task.Factory.StartNew(action, cancelToken, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
    }
}
