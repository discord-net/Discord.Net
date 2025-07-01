#if NET5_0_OR_GREATER
using Discord.API.Gateway;
using Discord.API.Voice;
using Discord.Audio;
using System.Collections.Generic;
using System.Linq;

namespace Discord.WebSocket.Diagnostics
{
    internal static class DiagnosticTags
    {
        internal static IEnumerable<KeyValuePair<string, object>> CreateSocketClientTags(DiscordSocketClient client) => [
            KeyValuePair.Create<string, object>("discord.client.shard_id", client.ShardId),
            KeyValuePair.Create<string, object>("discord.client.api_version", $"v{DiscordConfig.APIVersion}"),
            KeyValuePair.Create<string, object>("discord.client.gateway_url", client.ApiClient.GatewayUrl)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateEventTags(GatewayOpCode opCode, string type)
        {
            IEnumerable<KeyValuePair<string, object>> tags = [
                KeyValuePair.Create<string, object>("discord.event_op_code", opCode)
            ];
            if (!string.IsNullOrEmpty(type))
                tags = tags.Append(new KeyValuePair<string, object>("discord.event_op_type", type));
            return tags;
        }

        internal static IEnumerable<KeyValuePair<string, object>> CreateAudioClientTags(AudioClient client) => [
            KeyValuePair.Create<string, object>("discord.audio.client_id", client.ClientId),
            KeyValuePair.Create<string, object>("discord.guild_id", client.Guild.Id),
            KeyValuePair.Create<string, object>("discord.channel_id", client.ChannelId)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateAudioEventTags(VoiceOpCode opCode) => [
            KeyValuePair.Create<string, object>("discord.audio.event_op_code", opCode)
        ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateUdpTags(AudioClient client) => [
            KeyValuePair.Create<string, object>("discord.audio.client_port", client.ApiClient.UdpPort),
            KeyValuePair.Create<string, object>("discord.audio.server.remote_ip", client.ApiClient.UdpRemoteIp),
            KeyValuePair.Create<string, object>("discord.audio.server.remote_port", client.ApiClient.UdpRemotePort)
        ];
    }
}
#endif
