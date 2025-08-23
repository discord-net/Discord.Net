using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetOriginalWebhookMessage(
        RouteParameters.WebhookId WebhookId,
        RouteParameters.WebhookToken WebhookToken
    ) : IOperation, Expand<GetOriginalWebhookMessage, GetOriginalWebhookMessage>
    {
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId), typeof(RouteParameters.WebhookToken)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId, WebhookToken];
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}/messages/@original";
        public static string OperationId => "get_original_webhook_message";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}/messages/@original{QueryStrings.Build(("thread_id", ThreadId.ToNullable()))}";
    }
}