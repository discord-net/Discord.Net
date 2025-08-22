namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteWebhookByToken(
        Snowflake WebhookId,
        string WebhookToken
    ) : IOperation
    {
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}";
        public static string OperationId => "delete_webhook_by_token";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}";
    }
}