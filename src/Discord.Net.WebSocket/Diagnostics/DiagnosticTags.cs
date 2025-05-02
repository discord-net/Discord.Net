#if NET5_0_OR_GREATER
using Discord.Audio;
using System.Collections.Generic;

namespace Discord.WebSocket.Diagnostics
{
    internal static class DiagnosticTags
    {
        internal static IEnumerable<KeyValuePair<string, object>> Create(DiscordSocketClient client) => [
            KeyValuePair.Create<string, object>("client.shard_id", client.ShardId),
            KeyValuePair.Create<string, object>("client.api_version", $"v{DiscordConfig.APIVersion}")
        ];

        internal static IEnumerable<KeyValuePair<string, object>> Create(string type, DiscordSocketClient client) => [
            ..Create(client),
            KeyValuePair.Create<string, object>("event.type", type)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> Create(AudioClient client) => [
            KeyValuePair.Create<string, object>("client.id", client.ClientId)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateUdpTags(AudioClient client) => [
            ..Create(client),
            KeyValuePair.Create<string, object>("client.port", client.ApiClient.UdpPort),
            KeyValuePair.Create<string, object>("server.remote_ip", client.ApiClient.UdpRemoteIp),
            KeyValuePair.Create<string, object>("server.remote_port", client.ApiClient.UdpRemotePort)
        ];
    }
}
#endif
