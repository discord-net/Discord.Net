namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteChannelPermissionOverwrite(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.OverwriteId OverwriteId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.OverwriteId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, OverwriteId];
    
        public static string Path => @"/channels/{channel_id}/permissions/{overwrite_id}";
        public static string OperationId => "delete_channel_permission_overwrite";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/permissions/{OverwriteId}";
    }
}