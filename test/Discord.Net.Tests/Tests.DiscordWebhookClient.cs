using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests the <see cref="DiscordWebhookClient.ParseWebhookUrl(string, out ulong, out string)"/> function.
    /// </summary>
    public class DiscordWebhookClientTests
    {
        [Theory]
        [InlineData("https://discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK",
            123412347732897802, "_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // ptb, canary, etc will have slightly different urls
        [InlineData("https://ptb.discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK",
            123412347732897802, "_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        [InlineData("https://canary.discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK",
            123412347732897802, "_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // don't care about https
        [InlineData("http://canary.discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK",
            123412347732897802, "_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // this is the minimum that the regex cares about
        [InlineData("discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK",
            123412347732897802, "_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        public void TestWebhook_Valid(string webhookurl, ulong expectedId, string expectedToken)
        {
            DiscordWebhookClient.ParseWebhookUrl(webhookurl, out ulong id, out string token);

            Assert.Equal(expectedId, id);
            Assert.Equal(expectedToken, token);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void TestWebhook_Null(string webhookurl)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DiscordWebhookClient.ParseWebhookUrl(webhookurl, out ulong id, out string token);
            });
        }

        [Theory]
        [InlineData("123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // trailing slash
        [InlineData("https://discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK/")]
        public void TestWebhook_Invalid(string webhookurl)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DiscordWebhookClient.ParseWebhookUrl(webhookurl, out ulong id, out string token);
            });
        }
    }
}
