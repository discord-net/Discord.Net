using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ExecuteWebhook(
        RouteParameters.WebhookId WebhookId,
        RouteParameters.WebhookToken WebhookToken
    ) : IOperation, Expand<ExecuteWebhook, ExecuteWebhook>
    {
        public Optional<bool> Wait { get; init; }
        public Optional<Snowflake> ThreadId { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId), typeof(RouteParameters.WebhookToken)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId, WebhookToken];
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}";
        public static string OperationId => "execute_webhook";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}{QueryStrings.Build(("wait", Wait.ToNullable()), ("thread_id", ThreadId.ToNullable()))}";
    }
}