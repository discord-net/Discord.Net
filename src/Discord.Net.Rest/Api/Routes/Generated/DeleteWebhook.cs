using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteWebhook(
        RouteParameters.WebhookId WebhookId
    ) : IOperation, Expand<DeleteWebhook, DeleteWebhook>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.WebhookId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [WebhookId];
    
        public static string Path => @"/webhooks/{webhook_id}";
        public static string OperationId => "delete_webhook";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/webhooks/{WebhookId}";
    }
}