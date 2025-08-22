namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateWebhookMessage(
        RouteParameters.WebhookId WebhookId,
        RouteParameters.WebhookToken WebhookToken,
        RouteParameters.MessageId MessageId
    ) : IOperation
    {
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId), typeof(RouteParameters.WebhookToken), typeof(RouteParameters.MessageId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId, WebhookToken, MessageId];
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}/messages/{message_id}";
        public static string OperationId => "update_webhook_message";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}/messages/{MessageId}{QueryStrings.Build(("thread_id", ThreadId.ToNullable()))}";
    }
}