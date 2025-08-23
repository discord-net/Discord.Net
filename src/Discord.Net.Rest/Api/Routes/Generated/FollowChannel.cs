using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record FollowChannel(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<FollowChannel, FollowChannel>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/followers";
        public static string OperationId => "follow_channel";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/followers";
    }
}