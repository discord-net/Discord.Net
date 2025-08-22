namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateOriginalWebhookMessage(
        RouteParameters.WebhookId WebhookId,
        RouteParameters.WebhookToken WebhookToken
    ) : IOperation
    {
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId), typeof(RouteParameters.WebhookToken)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId, WebhookToken];
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}/messages/@original";
        public static string OperationId => "update_original_webhook_message";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}/messages/@original{QueryStrings.Build(("thread_id", ThreadId.ToNullable()))}";
    }
}