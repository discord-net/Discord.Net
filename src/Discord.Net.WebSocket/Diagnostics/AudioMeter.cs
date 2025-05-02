using Discord.Audio;

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
        private static readonly Histogram<double> _udpLatency;

        private static readonly Counter<long> _audioBytesReceived;
        private static readonly Counter<long> _audioBytesSent;

        static AudioMeter()
        {
#if NET7_0_OR_GREATER
            _audioConnections = _meter.CreateUpDownCounter<int>(
                name: "audio.connections_count",
                unit: "Connections",
                description: "The amount of UDP audio connections currently active.");
#endif
            _udpLatency = _meter.CreateHistogram<double>(
                name: "udp.connections.latency",
                unit: "Seconds",
                description: "The latency of the open UDP audio connections.");

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

        internal static void RecordUdpLatency(double seconds, AudioClient client) { }

        internal static void RecordBytesReceived(int amount, AudioClient client) { }

        internal static void RecordBytesSent(int amount, AudioClient client) { }
#endif
    }
}
