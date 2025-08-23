using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateMessage(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<CreateMessage, CreateMessage>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/messages";
        public static string OperationId => "create_message";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages";
    }
}