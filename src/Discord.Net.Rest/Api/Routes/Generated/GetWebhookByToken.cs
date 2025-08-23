using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetWebhookByToken(
        RouteParameters.WebhookId WebhookId,
        RouteParameters.WebhookToken WebhookToken
    ) : IOperation, Expand<GetWebhookByToken, GetWebhookByToken>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId), typeof(RouteParameters.WebhookToken)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId, WebhookToken];
    
        public static string Path => @"/webhooks/{webhook_id}/{webhook_token}";
        public static string OperationId => "get_webhook_by_token";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}/{WebhookToken}";
    }
}