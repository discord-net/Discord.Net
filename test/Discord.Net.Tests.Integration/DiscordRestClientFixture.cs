using Discord.Rest;
using System;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Test fixture type for integration tests which sets up the client from
    ///     the token provided in environment variables.
    /// </summary>
    public class DiscordRestClientFixture : IDisposable
    {
        public DiscordRestClient Client { get; private set; }

        public DiscordRestClientFixture()
        {
            var token = Environment.GetEnvironmentVariable("DNET_TEST_TOKEN", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("The DNET_TEST_TOKEN environment variable was not provided.");
            Client = new DiscordRestClient(new DiscordRestConfig()
            {
                LogLevel = LogSeverity.Debug,
                DefaultRetryMode = RetryMode.AlwaysRetry
            });
            Client.LoginAsync(TokenType.Bot, token).Wait();
        }

        public void Dispose()
        {
            Client.LogoutAsync().Wait();
            Client.Dispose();
        }
    }
}
