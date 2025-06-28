using System;

#if NET5_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics;
#endif

namespace Discord.WebSocket.Diagnostics
{
    internal static class SocketActivity
    {
#if NET5_0_OR_GREATER
        private static readonly ActivitySource _source = new("Discord.Net.WebSocket", typeof(DiagnosticTags).Assembly.GetName().Version!.ToString());

        internal static Activity StartSocketDispatchActivity(int? seq, string type, DiscordSocketClient client)
        {
            Activity.Current = null;     // This activity doesn't have a parent so it have to be explicitly set

            IEnumerable<KeyValuePair<string, object>> tags = [
                .. DiagnosticTags.CreateSocketClientTags(client),
                .. DiagnosticTags.CreateEventTags(seq, type),
            ];
            return _source.StartActivity($"process {type}", ActivityKind.Consumer, null, tags: tags);
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
        internal static IDisposable StartSocketDispatchActivity(int? seq, string type, DiscordSocketClient client) => null;

        internal static void AddExceptionToActivity(this IDisposable activity, Exception ex) { }
#endif
    }
}
