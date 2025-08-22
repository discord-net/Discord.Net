namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateOriginalWebhookMessage(
        Snowflake WebhookId,
        string WebhookToken
    ) : IOperation
    {
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}/messages/@original";
        public static string OperationId => "update_original_webhook_message";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}/messages/@original{QueryStrings.Build(("thread_id", ThreadId.ToNullable()))}";
    }
}