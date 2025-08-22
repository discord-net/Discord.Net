namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ExecuteWebhook(
        Snowflake WebhookId,
        string WebhookToken
    ) : IOperation
    {
        public Optional<bool> Wait { get; init; }
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}";
        public static string OperationId => "execute_webhook";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}{QueryStrings.Build(("wait", Wait.ToNullable()), ("thread_id", ThreadId.ToNullable()))}";
    }
}