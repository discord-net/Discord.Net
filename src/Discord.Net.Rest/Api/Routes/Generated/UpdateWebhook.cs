namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateWebhook(
        RouteParameters.WebhookId WebhookId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId];
    
        public static string Path => @"/webhooks/{webhook_id}";
        public static string OperationId => "update_webhook";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}";
    }
}