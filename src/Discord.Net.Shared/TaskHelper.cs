using System;
using System.Threading.Tasks;

namespace Discord
{
    internal static class TaskHelper
    {
        public static Task CompletedTask { get; }
        static TaskHelper()
        {
#if DOTNET54
            CompletedTask = Task.CompletedTask;
#else
            CompletedTask = Task.Delay(0);
#endif
        }

        public static Func<Task> ToAsync(Action action)
        {
            return () =>
            {
                action(); return CompletedTask;
            };
        }
        public static Func<T, Task> ToAsync<T>(Action<T> action)
        {
            return x =>
            {
                action(x); return CompletedTask;
            };
        }
    }
}
