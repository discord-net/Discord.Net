using Discord.API.Gateway;
using System;

#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
#endif

namespace Discord.WebSocket.Diagnostics
{
    internal static class SocketMeter
    {
#if NET6_0_OR_GREATER
        private readonly static Meter _meter = new(Options.SourceName, Options.Version);

#if NET7_0_OR_GREATER
        private readonly static UpDownCounter<int> _clientShards;

        private readonly static UpDownCounter<int> _socketConnections;
#endif
        private readonly static Histogram<double> _socketConnectionsLatency;

        private readonly static Counter<long> _socketEvents;
        private readonly static Counter<int> _socketEventExceptions;
        private readonly static Counter<long> _socketDispatches;
        private readonly static Counter<int> _socketDispatchesExceptions;
        private readonly static Histogram<double> _socketDispatchesDuration;

#if NET9_0_OR_GREATER
        /* 
         * OTel bucket boundary recommendation for 'http.request.duration':
         * [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
         * (https://github.com/open-telemetry/semantic-conventions/blob/release/v1.23.x/docs/http/http-metrics.md#metric-httpclientrequestduration)
         */
        private readonly static double[] _histogramBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.125, 0.15, 0.175, 0.2, 0.225, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];     // Higher resolution in the area from 0.1 to 0.25 in 0.025 steps
#endif

        static SocketMeter()
        {
#if NET7_0_OR_GREATER
            _clientShards = _meter.CreateUpDownCounter<int>(
                name: "client.shards_count",
                unit: "Shards",
                description: "The amount of shards that currently exists.");

            _socketConnections = _meter.CreateUpDownCounter<int>(
                name: "socket.connections_count",
                unit: "Connections",
                description: "The total amount of WebSocket connections currently connected (should match the amount of shards).");
#endif
            _socketConnectionsLatency = _meter.CreateHistogram<double>(
                name: "socket.connections.latency",
                unit: "Seconds",
                description: "The latency of the open WebSocket connections."
#if NET9_0_OR_GREATER
                , advice: new() { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );

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
                description: "The handling duration of dispatches (like 'READY' or 'INTERACTION_CREATE') received from the gateway."
#if NET9_0_OR_GREATER
                , advice: new() { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );
        }

        internal static void AddClientShards(int shards, DiscordSocketConfig config)
        {
#if NET7_0_OR_GREATER
            _clientShards.Add(shards, [.. Options.CreateTags(config)]);
#endif
        }

        internal static void AddSocketConnections(int connections, DiscordSocketConfig config)
        {
#if NET7_0_OR_GREATER
            _socketConnections.Add(connections, [.. Options.CreateTags(config)]);
#endif
        }

        internal static void RecordSocketLatency(double seconds, DiscordSocketConfig config)
        {
            _socketConnectionsLatency.Record(seconds, [.. Options.CreateTags(config)]);
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
        internal static void AddClientShards(int shards, DiscordSocketConfig config) { }

        internal static void AddSocketConnections(int connections, DiscordSocketConfig config) { }

        internal static void RecordSocketLatency(double seconds, DiscordSocketConfig config) { }

        internal static void RecordSocketEvent(GatewayOpCode opCode, string type, DiscordSocketConfig config) { }

        internal static void RecordSocketEventException(Exception ex, GatewayOpCode opCode, string type, DiscordSocketConfig config) { }

        internal static void RecordSocketDispatch(TimeSpan duration, string type, DiscordSocketConfig config) { }
#endif
    }
}
