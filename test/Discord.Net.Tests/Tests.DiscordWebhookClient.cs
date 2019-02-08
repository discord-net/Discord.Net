using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests that the <see cref=""/>
    /// </summary>
    public class DiscordWebhookClientTests
    {
        [Theory]
        [InlineData("https://discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // ptb, canary, etc will have slightly different urls
        [InlineData("https://ptb.discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        [InlineData("https://canary.discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // don't care about https
        [InlineData("http://canary.discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        // this is the minimum that the regex cares about
        [InlineData("discordapp.com/api/webhooks/123412347732897802/_abcde123456789-ABCDEFGHIJKLMNOP12345678-abcdefghijklmnopABCDEFGHIJK")]
        public void TestWebhook_Valid(string webhookurl)
        {
            try
            {
                _ = new DiscordWebhookClient(webhookurl);
            }
            catch (InvalidOperationException)
            {
                // ignore, thrown because webhook urls are invalid
            }
            
            // pass if no exception thrown
            Assert.True(true);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void TestWebhook_Null(string webhookurl)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new DiscordWebhookClient(webhookurl);
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
                _ = new DiscordWebhookClient(webhookurl);
            });
        }
    }
}
