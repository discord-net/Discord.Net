using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Discord;
using Discord.Rest;

namespace Discord.Tests.Rest
{
    public class LoginTests : IClassFixture<RestFixture>
    {
        DiscordRestClient client;

        public LoginTests(RestFixture fixture)
        {
            client = fixture.Client;
        }
    }
}
