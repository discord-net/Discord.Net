using System;

namespace Discord
{
    internal static class EventExtensions
    {
        public static void Raise(this EventHandler eventHandler, object sender)
            => eventHandler?.Invoke(sender, EventArgs.Empty);
        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T eventArgs)
            where T : EventArgs
            => eventHandler?.Invoke(sender, eventArgs);
    }
}
