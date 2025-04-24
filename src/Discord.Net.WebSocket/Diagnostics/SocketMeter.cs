using Discord.API.Gateway;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#if NET6_0_OR_GREATER
using System.Diagnostics.Metrics;
#endif

namespace Discord.WebSocket.Diagnostics
{
    internal static class SocketMeter
    {
#if NET6_0_OR_GREATER
        private readonly static Meter _meter = new(Options.SourceName, Options.Version);

        private readonly static Counter<long> _socketEvents;
        private readonly static Counter<int> _socketEventExceptions;
        private readonly static Counter<long> _socketDispatches;
        private readonly static Counter<int> _socketDispatchesExceptions;
        private readonly static Histogram<double> _socketDispatchesDuration;

        static SocketMeter()
        {
            _socketEvents = _meter.CreateCounter<long>(
                name: "socket.events_count",
                unit: "Events",
                description: "The total amount of events sent by the gateway since the application is running.");
            _socketEventExceptions = _meter.CreateCounter<int>(
                name: "socket.events.exceptions_count",
                unit: "Exceptions",
                description: "The amount of exceptions occurred while event procession.");
            _socketDispatches = _meter.CreateCounter<long>(
                name: "socket.dispatches_count",
                unit: "Dispatches",
                description: "The total amount of dispatches (like 'READY' or 'INTERACTION_CREATE') sent by the gateway since the application is running.");
            _socketDispatchesExceptions = _meter.CreateCounter<int>(
                name: "socket.dispatches.exceptions_count",
                unit: "Exceptions",
                description: "The amount of exceptions occurred while handling dispatches (like 'READY' or 'INTERACTION_CREATE').");
            _socketDispatchesDuration = _meter.CreateHistogram<double>(
                name: "socket.dispatches.duration",
                unit: "Seconds",
                description: "The handling duration of dispatches (like 'READY' or 'INTERACTION_CREATE') received from the gateway.");
        }

        internal static void RecordSocketEvent(GatewayOpCode opCode, string type, DiscordSocketConfig config)
        {
            _socketEvents.Add(1, [..Options.CreateTags(opCode, type, config)]);
        }

        internal static void RecordSocketEventException(Exception ex, GatewayOpCode opCode, string type, DiscordSocketConfig config)
        {
            TagList tags = [
                .. Options.CreateTags(opCode, type, config),
                KeyValuePair.Create<string, object>("exception.type", ex.GetType().ToString()),
                KeyValuePair.Create<string, object>("exception.message", ex.Message),
                KeyValuePair.Create<string, object>("exception.stacktrace", ex.ToString()),
            ];

            _socketEventExceptions.Add(1, tags);
            if (opCode == GatewayOpCode.Dispatch)
                _socketDispatchesExceptions.Add(1, tags);
        }

        internal static void RecordSocketDispatch(TimeSpan duration, string type, DiscordSocketConfig config)
        {
            TagList tags = [..Options.CreateTags(GatewayOpCode.Dispatch, type, config)];
            _socketDispatches.Add(1, tags);
            _socketDispatchesDuration.Record(duration.TotalSeconds, tags);
        }
#else
        internal static void RecordSocketEvent(GatewayOpCode opCode, string type, DiscordSocketConfig config) { }

        internal static void RecordSocketEventException(Exception ex, GatewayOpCode opCode, string type, DiscordSocketConfig config) { }

        internal static void RecordSocketDispatch(TimeSpan duration, string type, DiscordSocketConfig config) { }
#endif
    }
}
