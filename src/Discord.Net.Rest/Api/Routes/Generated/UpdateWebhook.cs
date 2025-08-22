namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateWebhook(
        Snowflake WebhookId
    ) : IOperation
    {
        public static string Path => @"/webhooks/{webhook_id}";
        public static string OperationId => "update_webhook";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}";
    }
}