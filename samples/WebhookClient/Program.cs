using Discord;
using Discord.Webhook;
using System.Threading.Tasks;

namespace WebHookClient
{
    // This is a minimal example of using Discord.Net's Webhook Client
    // Webhooks are send-only components of Discord that allow you to make a POST request
    // To a channel specific URL to send a message to that channel.
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // The webhook url follows the format https://discord.com/api/webhooks/{id}/{token}
            // Because anyone with the webhook URL can use your webhook
            // you should NOT hard code the URL or ID + token into your application.
            using (var client = new DiscordWebhookClient("https://discord.com/api/webhooks/123/abc123"))
            {
                var embed = new EmbedBuilder
                {
                    Title = "Test Embed",
                    Description = "Test Description"
                };

                // Webhooks are able to send multiple embeds per message
                // As such, your embeds must be passed as a collection.
                await client.SendMessageAsync(text: "Send a message to this webhook!", embeds: new[] { embed.Build() });
            }
        }
    }
}
