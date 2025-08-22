namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateWebhookMessage(
        Snowflake WebhookId,
        string WebhookToken,
        Snowflake MessageId
    ) : IOperation
    {
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}/messages/{message_id}";
        public static string OperationId => "update_webhook_message";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}/messages/{MessageId}{QueryStrings.Build(("thread_id", ThreadId.ToNullable()))}";
    }
}