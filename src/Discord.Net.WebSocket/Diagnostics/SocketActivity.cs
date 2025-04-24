using Discord.API.Gateway;
using System;
using System.Diagnostics;

namespace Discord.WebSocket.Diagnostics
{
    internal static class SocketActivity
    {
#if NET5_0_OR_GREATER
        private static readonly ActivitySource _source = new(Options.SourceName, Options.Version);

        internal static Activity StartSocketDispatchActivity(string type, DiscordSocketConfig config)
        {
            return _source.StartActivity(
                "dispatch socket event",
                ActivityKind.Consumer,
                null,
                tags: Options.CreateTags(GatewayOpCode.Dispatch, type, config));
        }

        internal static void AddExceptionToActivity(this Activity activity, Exception ex)
        {
#if NET6_0_OR_GREATER
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
#endif
#if NET9_0_OR_GREATER
            activity.AddException(ex);
#else
            activity.AddEvent(new("exception", tags: new()
            {
                { "exception.type", ex.GetType().ToString() },
                { "exception.message", ex.Message },
                { "exception.stacktrace", ex.ToString() }
            }));
#endif
        }
#else
        internal static IDisposable StartSocketDispatchActivity(string type, DiscordSocketConfig config) => null;

        internal static void AddExceptionToActivity(this IDisposable activity, Exception ex) { }
#endif
    }
}
