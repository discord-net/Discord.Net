namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record JoinThread(
        RouteParameters.ChannelId ChannelId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/thread-members/@me";
        public static string OperationId => "join_thread";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/thread-members/@me";
    }
}