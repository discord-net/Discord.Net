using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.Net.Rest;
using Discord.Tests.Framework;

namespace Discord.Tests.Rest
{
    public class RestFixture
    {
        public DiscordRestClient Client { get; set; }

        public RestFixture()
        {
            var Config = new DiscordRestConfig()
            {
                RestClientProvider = new RestClientProvider(baseUrl => new MockRestClient(baseUrl))
            };
            Client = new DiscordRestClient();
        }
    }
}
