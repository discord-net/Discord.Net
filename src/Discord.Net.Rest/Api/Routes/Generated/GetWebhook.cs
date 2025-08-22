namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetWebhook(
        RouteParameters.WebhookId WebhookId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId];
    
        public static string Path => @"/webhooks/{webhook_id}";
        public static string OperationId => "get_webhook";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}";
    }
}