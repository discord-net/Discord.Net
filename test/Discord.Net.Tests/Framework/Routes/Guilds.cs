using System.Collections.Generic;
using GuildMocks = Discord.Tests.Framework.Mocks.Rest.Guilds;

namespace Discord.Tests.Framework.Routes
{
    public static class Guilds
    {
        public static object DiscordApi(string json, IReadOnlyDictionary<string, string> requestHeaders) =>
            GuildMocks.DiscordApi;
    }
}
