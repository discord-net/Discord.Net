namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGroupDmUser(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.UserId UserId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, UserId];
    
        public static string Path => @"/channels/{channel_id}/recipients/{user_id}";
        public static string OperationId => "delete_group_dm_user";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/recipients/{UserId}";
    }
}