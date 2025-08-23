using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record AddGroupDmUser(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.UserId UserId
    ) : IOperation, Expand<AddGroupDmUser, AddGroupDmUser>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, UserId];
    
        public static string Path => @"/channels/{channel_id}/recipients/{user_id}";
        public static string OperationId => "add_group_dm_user";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/recipients/{UserId}";
    }
}