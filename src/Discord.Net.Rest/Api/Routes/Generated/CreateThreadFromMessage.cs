using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateThreadFromMessage(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId
    ) : IOperation, Expand<CreateThreadFromMessage, CreateThreadFromMessage>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId];
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/threads";
        public static string OperationId => "create_thread_from_message";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/threads";
    }
}