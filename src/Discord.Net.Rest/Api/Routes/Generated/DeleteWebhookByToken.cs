using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteWebhookByToken(
        RouteParameters.WebhookId WebhookId,
        RouteParameters.WebhookToken WebhookToken
    ) : IOperation, Expand<DeleteWebhookByToken, DeleteWebhookByToken>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId), typeof(RouteParameters.WebhookToken)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId, WebhookToken];
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}";
        public static string OperationId => "delete_webhook_by_token";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}";
    }
}