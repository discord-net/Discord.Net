using System;
using Discord.Audio;
using Discord.API.Voice;

#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
#endif

namespace Discord.WebSocket.Diagnostics
{
    internal static class AudioMeter
    {
#if NET6_0_OR_GREATER
        private static readonly Meter _meter = new("Discord.Net.Audio", typeof(DiagnosticTags).Assembly.GetName().Version.ToString());

#if NET7_0_OR_GREATER
        private static readonly UpDownCounter<int> _audioConnections;
#endif
        private static readonly Histogram<double> _socketLatency;
        private static readonly Histogram<double> _udpLatency;

        private static readonly Counter<long> _audioBytesReceived;
        private static readonly Counter<long> _audioBytesSent;

        private static readonly Counter<long> _socketEventsSentCount;
        private static readonly Counter<long> _socketEventsReceivedCount;
        private static readonly Histogram<double> _socketEventsReceivedDuration;
        private static readonly Counter<int> _socketEventsReceivedExceptions;

#if NET9_0_OR_GREATER
        /* 
         * OTel bucket boundary recommendation for 'http.request.duration':
         * [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
         * (https://github.com/open-telemetry/semantic-conventions/blob/release/v1.23.x/docs/http/http-metrics.md#metric-httpclientrequestduration)
         */
        private readonly static double[] _histogramBoundaries = [0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.125, 0.15, 0.175, 0.2, 0.225, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10];     // Higher resolution in the area from 0.1 to 0.25 in 0.025 steps
#endif

        static AudioMeter()
        {
#if NET7_0_OR_GREATER
            _audioConnections = _meter.CreateUpDownCounter<int>(
                name: "audio.connections_count",
                unit: "Connections",
                description: "The amount of both audio WebSocket and UDP connections currently active.");
#endif

            _socketLatency = _meter.CreateHistogram<double>(
                name: "socket.connections.latency",
                unit: "Seconds",
                description: "The latency of the active audio WebSocket connections."
#if NET9_0_OR_GREATER
                , advice: new() { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );
            _socketEventsSentCount = _meter.CreateCounter<long>(
                name: "socket.events_sent.count",
                unit: "Events",
                description: "The amount of events sent to the audio gateway.");
            _socketEventsReceivedCount = _meter.CreateCounter<long>(
                name: "socket.events_received.count",
                unit: "Events",
                description: "The amount of events received from the audio gateway.");
            _socketEventsReceivedDuration = _meter.CreateHistogram<double>(
                name: "socket.events_received.duration",
                unit: "Seconds",
                description: "The duration it took to process events received from the audio gateway.");
            _socketEventsReceivedExceptions = _meter.CreateCounter<int>(
                name: "socket.event_received.exception_count",
                unit: "Exceptions",
                description: "The amount of exceptions occurred while processing events received from the audio gateway.");

            _udpLatency = _meter.CreateHistogram<double>(
                name: "udp.connections.latency",
                unit: "Seconds",
                description: "The latency of the open UDP audio connections."
#if NET9_0_OR_GREATER
                , advice: new() { HistogramBucketBoundaries = _histogramBoundaries }
#endif
                );
            _audioBytesReceived = _meter.CreateCounter<long>(
                name: "udp.bytes_received",
                unit: "Bytes",
                description: "The total amount of bytes received from every UDP audio connection.");
            _audioBytesSent = _meter.CreateCounter<long>(
                name: "udp.bytes_sent",
                unit: "Bytes",
                description: "The total amount of bytes sent by every UDP audio connection.");
        }

        internal static void AddAudioConnections(int connections, AudioClient client)
        {
#if NET7_0_OR_GREATER
            _audioConnections.Add(connections);
#endif
        }

        internal static void RecordSocketLatency(double seconds, AudioClient client)
        {
            _socketLatency.Record(seconds, [.. DiagnosticTags.Create(client)]);
        }

        internal static void RecordSocketEventSent(VoiceOpCode op, AudioClient client)
        {
            _socketEventsSentCount.Add(1, [.. DiagnosticTags.Create(op, client)]);
        }

        internal static void RecordSocketEventReceived(VoiceOpCode op, TimeSpan duration, AudioClient client)
        {
            TagList tags = [
                .. DiagnosticTags.Create(client),
                KeyValuePair.Create<string, object>("event.op_code", op)
            ];
            _socketEventsReceivedCount.Add(1, tags);
            _socketEventsReceivedDuration.Record(duration.TotalSeconds, tags);
        }

        internal static void RecordSocketEventException(VoiceOpCode op, Exception ex, AudioClient client)
        {
            _socketEventsReceivedExceptions.Add(1, [
                .. DiagnosticTags.Create(op, client),
                KeyValuePair.Create<string, object>("exception.type", ex.GetType().ToString()),
                KeyValuePair.Create<string, object>("exception.message", ex.Message),
                KeyValuePair.Create<string, object>("exception.stacktrace", ex.ToString())
            ]);
        }

        internal static void RecordUdpLatency(double seconds, AudioClient client)
        {
            _udpLatency.Record(seconds, [.. DiagnosticTags.CreateUdpTags(client)]);
        }

        internal static void RecordBytesReceived(int amount, AudioClient client)
        {
            _audioBytesReceived.Add(amount, [.. DiagnosticTags.CreateUdpTags(client)]);
        }

        internal static void RecordBytesSent(int amount, AudioClient client)
        {
            _audioBytesSent.Add(amount, [.. DiagnosticTags.CreateUdpTags(client)]);
        }
#else
        internal static void AddAudioConnections(int connections, AudioClient client) { }

        internal static void RecordSocketLatency(double seconds, AudioClient client) { }

        internal static void RecordSocketEventSent(VoiceOpCode op, AudioClient client) { }

        internal static void RecordSocketEventReceived(VoiceOpCode op, TimeSpan duration, AudioClient client) { }

        internal static void RecordSocketEventException(VoiceOpCode op, Exception ex, AudioClient client) { }

        internal static void RecordUdpLatency(double seconds, AudioClient client) { }

        internal static void RecordBytesReceived(int amount, AudioClient client) { }

        internal static void RecordBytesSent(int amount, AudioClient client) { }
#endif
    }
}
