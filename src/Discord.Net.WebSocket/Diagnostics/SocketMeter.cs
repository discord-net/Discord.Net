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
        private static readonly Meter _meter = new("Discord.Net.WebSocket", typeof(DiagnosticTags).Assembly.GetName().Version!.ToString());

#if NET7_0_OR_GREATER
        private static readonly BufferedUpDownCounter _clientShards;     // Buffering is especially here required because Add gets called so early where the instrument isn't enabled yet.

        private static readonly BufferedUpDownCounter _socketConnections;
#endif
        private static readonly Counter<long> _socketReconnects;
        private static readonly Histogram<double> _socketConnectionsLatency;

        private static readonly Counter<long> _socketEvents;
        private static readonly Histogram<double> _socketEventsDuration;
        private static readonly Counter<int> _socketEventsExceptions;

#if NET9_0_OR_GREATER
        /*
         * OTel bucket boundary recommendation for 'http.request.duration':
         * [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
         * (https://github.com/open-telemetry/semantic-conventions/blob/release/v1.23.x/docs/http/http-metrics.md#metric-httpclientrequestduration)
         */
        private static readonly double[] _histogramBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.125, 0.15, 0.175, 0.2, 0.225, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];     // Higher resolution in the area from 0.1 to 0.25 in 0.025 steps
#endif

        static SocketMeter()
        {
            // Shard metrics
#if NET7_0_OR_GREATER
            _clientShards = new BufferedUpDownCounter(_meter.CreateUpDownCounter<int>(
                name: "discord.shards_count",
                unit: "Shards",
                description: "The amount of shards that currently exists."));

            // Socket client metrics
            _socketConnections = new BufferedUpDownCounter(_meter.CreateUpDownCounter<int>(
                name: "discord.socket.connections_count",
                unit: "Connections",
                description: "The total amount of WebSocket connections currently connected (should match the amount of 'discord.shards_count')."));
#endif
           _socketReconnects = _meter.CreateCounter<long>(
               name: "discord.socket.reconnects_count",
               unit: "Reconnects",
               description: "The amount of WebSocket connections reconnecting.");
            _socketConnectionsLatency = _meter.CreateHistogram<double>(
                name: "discord.socket.latency",
                unit: "Seconds",
                description: "The latency of the open WebSocket connections."
#if NET9_0_OR_GREATER
                , advice: new InstrumentAdvice<double> { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );

            // Socket client event metrics
            _socketEvents = _meter.CreateCounter<long>(
                name: "discord.events.received_count",
                unit: "Events",
                description: "The total amount of events received from the gateway since the application has startet.");
            _socketEventsDuration = _meter.CreateHistogram<double>(
                name: "discord.events.duration",
                unit: "Seconds",
                description: "The duration to dispatch events received from the gateway."
#if NET9_0_OR_GREATER
                , advice: new InstrumentAdvice<double> { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );
            _socketEventsExceptions = _meter.CreateCounter<int>(
                name: "discord.events.exceptions_count",
                unit: "Exceptions",
                description: "The amount of exceptions occurred while dispatching dispatches sent by the gateway.");
        }

        internal static void AddClientShards(int shards, DiscordSocketClient client)
        {
#if NET7_0_OR_GREATER
            _clientShards.Add(shards, [.. DiagnosticTags.CreateSocketClientTags(client)]);
#endif
        }

        internal static void AddSocketConnections(int connections, DiscordSocketClient client)
        {
#if NET7_0_OR_GREATER
            _socketConnections.Add(connections, [.. DiagnosticTags.CreateSocketClientTags(client)]);
#endif
        }

        internal static void AddSocketReconnect(DiscordSocketClient client)
        {
            _socketReconnects.Add(1, [.. DiagnosticTags.CreateSocketClientTags(client)]);
        }

        internal static void RecordConnectionLatency(double seconds, DiscordSocketClient client)
        {
            _socketConnectionsLatency.Record(seconds, [.. DiagnosticTags.CreateSocketClientTags(client)]);
        }

        internal static void RecordSocketEventException(Exception ex, int? seq, string type, DiscordSocketClient client)
        {
            TagList tags = [
                .. DiagnosticTags.CreateSocketClientTags(client),
                .. DiagnosticTags.CreateEventTags(seq, type),
                KeyValuePair.Create<string, object>("exception.type", ex.GetType().ToString()),
                KeyValuePair.Create<string, object>("exception.message", ex.Message),
                KeyValuePair.Create<string, object>("exception.stacktrace", ex.ToString()),
            ];
            _socketEventsExceptions.Add(1, tags);
        }

        internal static void RecordSocketEvent(TimeSpan duration, int? seq, string type, DiscordSocketClient client)
        {
            TagList tags = [
                .. DiagnosticTags.CreateSocketClientTags(client),
                .. DiagnosticTags.CreateEventTags(seq, type)
            ];

            _socketEvents.Add(1, tags);
            _socketEventsDuration.Record(duration.TotalSeconds, tags);
        }
#else
        internal static void AddClientShards(int shards, DiscordSocketClient client) { }

        internal static void AddSocketConnections(int connections, DiscordSocketClient client) { }

        internal static void AddSocketReconnect(DiscordSocketClient client) { }

        internal static void RecordConnectionLatency(double seconds, DiscordSocketClient client) { }

        internal static void RecordSocketEventException(Exception ex, int? seq, string type, DiscordSocketClient client) { }

        internal static void RecordSocketEvent(TimeSpan duration, int? seq, string type, DiscordSocketClient client) { }
#endif
    }
}
