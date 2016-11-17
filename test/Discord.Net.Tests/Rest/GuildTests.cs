using System.Threading.Tasks;
using Xunit;
using Discord.Rest;
using Contracts = Discord.Tests.Framework.Routes.Contracts;
using Mocks = Discord.Tests.Framework.Mocks.Rest.Guilds;
using RoleMocks = Discord.Tests.Framework.Mocks.Rest.Roles;

namespace Discord.Tests.Rest
{
    public class GuildTests : IClassFixture<RestFixture>
    {
        public GuildTests(RestFixture fixture)
        {
            _client = fixture.Client;
            _client.LoginAsync(TokenType.Bot, Contracts.BotToken).GetAwaiter().GetResult();
        }

        private DiscordRestClient _client;

        [Fact]
        public async Task GetGuild()
        {
            var guild = await _client.GetGuildAsync(81384788765712384);
            Assert.Equal(Mocks.DiscordApi.Id, guild.Id);
            Assert.Equal(Mocks.DiscordApi.Name, guild.Name);
            Assert.Equal(Mocks.DiscordApi.OwnerId, guild.OwnerId);
            Assert.Equal(Mocks.DiscordApi.MfaLevel, guild.MfaLevel);
            Assert.Equal(Mocks.DiscordApi.VerificationLevel, guild.VerificationLevel);
            Assert.Equal(Mocks.DiscordApi.Roles.Length, guild.Roles.Count);
            Assert.Equal(Mocks.DiscordApi.AFKTimeout, guild.AFKTimeout);
            Assert.Equal(Mocks.DiscordApi.DefaultMessageNotifications, guild.DefaultMessageNotifications);
            Assert.Equal(Mocks.DiscordApi.EmbedChannelId.GetValueOrDefault(), guild.EmbedChannelId);
            Assert.Equal(Mocks.DiscordApi.EmbedEnabled, guild.IsEmbeddable);
        }
        [Fact]
        public async Task GetInvalidGuild()
        {
            var guild = await _client.GetGuildAsync(1);
            Assert.Null(guild);
        }
    }
}
