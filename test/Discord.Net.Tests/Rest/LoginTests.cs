using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Discord;
using Discord.Rest;
using Routes = Discord.Tests.Framework.Routes.Users;
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
            await client.LoginAsync(TokenType.User, Routes.UserToken);
        }
        [Fact]
        public async Task LoginAsUserWithInvalidToken()
        {
            var client = fixture.Client;
            await Assert.ThrowsAsync<ArgumentException>(async () => await client.LoginAsync(TokenType.User, "token.invalid"));
        }
    }
}
