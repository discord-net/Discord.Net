using Discord.API.Gateway;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.WebSocket.Diagnostics
{
    internal static class Options
    {
        internal const string SourceName = "Discord.Net.WebSocket";
        internal static readonly string Version = typeof(Options).Assembly.GetName().Version.ToString();

#if NET5_0_OR_GREATER
        internal static IEnumerable<KeyValuePair<string, object>> CreateTags(DiscordSocketConfig config) => [
            KeyValuePair.Create<string, object>("client.gateway_host", config.GatewayHost ?? "/gateway"),
            KeyValuePair.Create<string, object>("client.shard_id", config.ShardId ?? 0)
            ];

        internal static IEnumerable<KeyValuePair<string, object>> CreateTags(GatewayOpCode opCode, string type, DiscordSocketConfig config) => [
            ..CreateTags(config),
            KeyValuePair.Create<string, object>("event.op_code", opCode),
            KeyValuePair.Create<string, object>("event.type", type)
            ];
#endif
    }

}
