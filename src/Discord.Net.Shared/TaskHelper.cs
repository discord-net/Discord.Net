using System;
using System.Threading.Tasks;

namespace Discord
{
    internal static class TaskHelper
    {
#if DOTNET54
        public static Task CompletedTask => Task.CompletedTask;                
#else
        public static Task CompletedTask => Task.Delay(0);
#endif

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
