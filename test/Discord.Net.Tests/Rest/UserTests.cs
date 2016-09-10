using System.Threading.Tasks;
using Xunit;
using Discord.Rest;
using Contracts = Discord.Tests.Framework.Routes.Contracts;
using Mocks = Discord.Tests.Framework.Mocks.Rest.Users;

namespace Discord.Tests.Rest
{
    public class UserTests : IClassFixture<RestFixture>
    {
        public UserTests(RestFixture fixture)
        {
            _client = fixture.Client;
            _client.LoginAsync(TokenType.Bot, Contracts.BotToken).GetAwaiter().GetResult();
        }

        private DiscordRestClient _client;

        [Fact]
        public async Task GetCurrentUser()
        {
            var user = await _client.GetCurrentUserAsync();
            Assert.Equal(Mocks.BotSelfUser.Id, user.Id);
            Assert.Equal(Mocks.BotSelfUser.Username.GetValueOrDefault(), user.Username);
            Assert.Equal(Mocks.BotSelfUser.Discriminator.GetValueOrDefault(), user.Discriminator);
            Assert.Equal(Mocks.BotSelfUser.Bot.GetValueOrDefault(), user.IsBot);
            Assert.Equal(Mocks.BotSelfUser.Email.GetValueOrDefault(), user.Email);
            Assert.Equal(Mocks.BotSelfUser.MfaEnabled.GetValueOrDefault(), user.IsMfaEnabled);
            Assert.Equal(Mocks.BotSelfUser.Verified.GetValueOrDefault(), user.IsVerified);
        }
        [Fact]
        public async Task GetUserById()
        {
            var user = await _client.GetUserAsync(66078337084162048);
            Assert.Equal(Mocks.PublicUser.Id, user.Id);
            Assert.Equal(Mocks.PublicUser.Username.GetValueOrDefault(), user.Username);
            Assert.Equal(Mocks.PublicUser.Discriminator.GetValueOrDefault(), user.Discriminator);
        }
        [Fact]
        public async Task GetInvalidUserById()
        {
            var user = await _client.GetUserAsync(1);
            Assert.Null(user);
        }
        [Fact]
        public async Task GetUserByTag()
        {
            var user = await _client.GetUserAsync("foxbot", "0282");
            Assert.Equal(Mocks.PublicUser.Id, user.Id);
            Assert.Equal(Mocks.PublicUser.Username.GetValueOrDefault(), user.Username);
            Assert.Equal(Mocks.PublicUser.Discriminator.GetValueOrDefault(), user.Discriminator);
        }
        [Fact]
        public async Task GetInvalidUserByTag()
        {
            var user = await _client.GetUserAsync("Voltana", "8252");
            Assert.Null(user);
        }
    }
}
