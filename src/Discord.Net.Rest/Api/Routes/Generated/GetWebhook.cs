using Discord.Models;
using Discord.Models.Rest.Api;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetWebhook(
        Snowflake WebhookId
    ) : IOperation
    {
        public static string Path => @"/webhooks/{webhook_id}";
        public static string OperationId => "get_webhook";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}";
    }
}