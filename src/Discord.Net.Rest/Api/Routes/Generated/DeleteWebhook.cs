namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteWebhook(
        Snowflake WebhookId
    ) : IOperation
    {
        public static string Path => @"/webhooks/{webhook_id}";
        public static string OperationId => "delete_webhook";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}";
    }
}