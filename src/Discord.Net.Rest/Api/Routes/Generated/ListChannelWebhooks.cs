namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListChannelWebhooks(
        RouteParameters.ChannelId ChannelId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/webhooks";
        public static string OperationId => "list_channel_webhooks";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/webhooks";
    }
}