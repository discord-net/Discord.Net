#if NET5_0_OR_GREATER
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
    }
}
#endif
