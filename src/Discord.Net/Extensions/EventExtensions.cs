using System;
using System.Threading.Tasks;

namespace Discord.Extensions
{
    internal static class EventExtensions
    {
        //TODO: Optimize these for if there is only 1 subscriber (can we do this?)
        public static async Task Raise(this Func<Task> eventHandler)
        {
            var subscriptions = eventHandler?.GetInvocationList();
            if (subscriptions != null)
            {
                for (int i = 0; i < subscriptions.Length; i++)
                    await (subscriptions[i] as Func<Task>).Invoke().ConfigureAwait(false);
            }
        }
        public static async Task Raise<T>(this Func<T, Task> eventHandler, T arg)
        {
            var subscriptions = eventHandler?.GetInvocationList();
            if (subscriptions != null)
            {
                for (int i = 0; i < subscriptions.Length; i++)
                    await (subscriptions[i] as Func<T, Task>).Invoke(arg).ConfigureAwait(false);
            }
        }
        public static async Task Raise<T1, T2>(this Func<T1, T2, Task> eventHandler, T1 arg1, T2 arg2)
        {
            var subscriptions = eventHandler?.GetInvocationList();
            if (subscriptions != null)
            {
                for (int i = 0; i < subscriptions.Length; i++)
                    await (subscriptions[i] as Func<T1, T2, Task>).Invoke(arg1, arg2).ConfigureAwait(false);
            }
        }
        public static async Task Raise<T1, T2, T3>(this Func<T1, T2, T3, Task> eventHandler, T1 arg1, T2 arg2, T3 arg3)
        {
            var subscriptions = eventHandler?.GetInvocationList();
            if (subscriptions != null)
            {
                for (int i = 0; i < subscriptions.Length; i++)
                    await (subscriptions[i] as Func<T1, T2, T3, Task>).Invoke(arg1, arg2, arg3).ConfigureAwait(false);
            }
        }
    }
}
