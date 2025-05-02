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
        private readonly static Meter _meter = new("Discord.Net.WebSocket", typeof(DiagnosticTags).Assembly.GetName().Version.ToString());

#if NET7_0_OR_GREATER
        private readonly static BufferedUpDownCounter _clientShards;     // Buffering is especially here required because Add gets called so early where the instrument isn't enabled yet.

        private readonly static BufferedUpDownCounter _socketConnections;
#endif
        private readonly static Histogram<double> _socketConnectionsLatency;

        private readonly static Counter<long> _socketEvents;
        private readonly static Histogram<double> _socketEventsDuration;
        private readonly static Counter<int> _socketEventsExceptions;

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
            _clientShards = new(_meter.CreateUpDownCounter<int>(
                name: "client.shards_count",
                unit: "Shards",
                description: "The amount of shards that currently exists."));

            _socketConnections = new(_meter.CreateUpDownCounter<int>(
                name: "socket.connections_count",
                unit: "Connections",
                description: "The total amount of WebSocket connections currently connected (should match the amount of shards)."));
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
                description: "The total amount of events sent by the gateway since the application has startet.");
            _socketEventsDuration = _meter.CreateHistogram<double>(
                name: "socket.events.duration",
                unit: "Seconds",
                description: "The duration to dispatch events received from the gateway."
#if NET9_0_OR_GREATER
                , advice: new() { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );
            _socketEventsExceptions = _meter.CreateCounter<int>(
                name: "socket.events.exceptions_count",
                unit: "Exceptions",
                description: "The amount of exceptions occurred while dispatching dispatches sent by the gateway.");
        }

        internal static void AddClientShards(int shards, DiscordSocketClient client)
        {
#if NET7_0_OR_GREATER
            _clientShards.Add(shards, [.. DiagnosticTags.Create(client)]);
#endif
        }

        internal static void AddSocketConnections(int connections, DiscordSocketClient client)
        {
#if NET7_0_OR_GREATER
            _socketConnections.Add(connections, [.. DiagnosticTags.Create(client)]);
#endif
        }

        internal static void RecordConnectionLatency(double seconds, DiscordSocketClient client)
        {
            _socketConnectionsLatency.Record(seconds, [.. DiagnosticTags.Create(client)]);
        }

        internal static void RecordSocketEventException(Exception ex, string type, DiscordSocketClient client)
        {
            TagList tags = [
                .. DiagnosticTags.Create(type, client),
                KeyValuePair.Create<string, object>("exception.type", ex.GetType().ToString()),
                KeyValuePair.Create<string, object>("exception.message", ex.Message),
                KeyValuePair.Create<string, object>("exception.stacktrace", ex.ToString()),
            ];
            _socketEventsExceptions.Add(1, tags);
        }

        internal static void RecordSocketEvent(TimeSpan duration, string type, DiscordSocketClient client)
        {
            TagList tags = [..DiagnosticTags.Create(type, client)];

            _socketEvents.Add(1, tags);
            _socketEventsDuration.Record(duration.TotalSeconds, tags);
        }
#else
        internal static void AddClientShards(int shards, DiscordSocketClient client) { }

        internal static void AddSocketConnections(int connections, DiscordSocketClient client) { }

        internal static void RecordConnectionLatency(double seconds, DiscordSocketClient client) { }

        internal static void RecordSocketEventException(Exception ex, string type, DiscordSocketClient client) { }

        internal static void RecordSocketEvent(TimeSpan duration, string type, DiscordSocketClient client) { }
#endif
    }
}
