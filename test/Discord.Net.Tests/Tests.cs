using System;
using Discord.Net;
using Discord.Rest;
using Xunit;

namespace Discord
{
    public partial class TestsFixture : IDisposable
    {
        private readonly TestConfig _config;
        private readonly CachedRestClient _cache;
        internal readonly DiscordRestClient _client;
        internal readonly RestGuild _guild;

        public TestsFixture()
        {
            _cache = new CachedRestClient();

            _config = TestConfig.LoadFile("./config.json");
            var config = new DiscordRestConfig
            {
                RestClientProvider = url =>
                {
                    _cache.SetUrl(url);
                    return _cache;
                }
            };
            _client = new DiscordRestClient(config);
            _client.LoginAsync(TokenType.Bot, _config.Token).Wait();

            MigrateAsync().Wait();
            _guild = _client.GetGuildAsync(_config.GuildId).Result;
        }

        public void Dispose()
        {
            _client.Dispose();
            _cache.Dispose();
        }
    }

    public partial class Tests : IClassFixture<TestsFixture>
    {
        private DiscordRestClient _client;
        private RestGuild _guild;

        public Tests(TestsFixture fixture)
        {
            _client = fixture._client;
            _guild = fixture._guild;
        }
    }
}