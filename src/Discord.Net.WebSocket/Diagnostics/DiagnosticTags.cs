#if NET5_0_OR_GREATER
using Discord.API.Voice;
using Discord.Audio;
using System.Collections.Generic;

namespace Discord.WebSocket.Diagnostics
{
    internal static class DiagnosticTags
    {
        internal static IEnumerable<KeyValuePair<string, object>> CreateSocketClientTags(DiscordSocketClient client) => [
            KeyValuePair.Create<string, object>("discord.client.shard_id", client.ShardId),
            KeyValuePair.Create<string, object>("discord.client.api_version", $"v{DiscordConfig.APIVersion}"),
            KeyValuePair.Create<string, object>("discord.client.gateway_url", client.ApiClient.GatewayUrl)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateEventTags(int? seq, string type) => [
            KeyValuePair.Create<string, object>("discord.event_type", type),
            KeyValuePair.Create<string, object>("discord.event_sequence", seq)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateAudioClientTags(AudioClient client) => [
            KeyValuePair.Create<string, object>("discord.audio.client_id", client.ClientId),
            KeyValuePair.Create<string, object>("discord.guild_id", client.Guild.Id),
            KeyValuePair.Create<string, object>("discord.channel_id", client.ChannelId)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateAudioEventTags(VoiceOpCode opCode) => [
            KeyValuePair.Create<string, object>("discord.audio.event_opCode", opCode)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateUdpTags(AudioClient client) => [
            KeyValuePair.Create<string, object>("discord.audio.client_port", client.ApiClient.UdpPort),
            KeyValuePair.Create<string, object>("discord.audio.server.remote_ip", client.ApiClient.UdpRemoteIp),
            KeyValuePair.Create<string, object>("discord.audio.server.remote_port", client.ApiClient.UdpRemotePort)
        ];
    }
}
#endif
