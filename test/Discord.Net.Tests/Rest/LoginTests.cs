using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Discord;
using Discord.Rest;
using Routes = Discord.Tests.Framework.Routes.Users;
using Contracts = Discord.Tests.Framework.Routes.Contracts;
using Discord.Net;

namespace Discord.Tests.Rest
{
    public class LoginTests : IClassFixture<RestFixture>
    {
        RestFixture fixture;

        public LoginTests(RestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task LoginAsUser()
        {
            var client = fixture.Client;
            await client.LoginAsync(TokenType.User, Contracts.UserToken);
        }
        [Fact]
        public async Task LoginAsUserWithInvalidToken()
        {
            var client = fixture.Client;
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.LoginAsync(TokenType.User, "token.invalid"));
        }
        [Fact]
        public async Task LoginAsBot()
        {
            var client = fixture.Client;
            await client.LoginAsync(TokenType.Bot, Contracts.BotToken);
        }
        [Fact]
        public async Task LoginAsBotWithInvalidToken()
        {
            var client = fixture.Client;
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.LoginAsync(TokenType.Bot, "token.invalid"));
        }
        [Fact]
        public async Task LoginAsBearer()
        {
            var client = fixture.Client;
            await client.LoginAsync(TokenType.Bearer, Contracts.BearerToken);
        }
        [Fact]
        public async Task LoginAsBearerWithInvalidToken()
        {
            var client = fixture.Client;
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.LoginAsync(TokenType.Bearer, "token.invalid"));
        }
    }
}
