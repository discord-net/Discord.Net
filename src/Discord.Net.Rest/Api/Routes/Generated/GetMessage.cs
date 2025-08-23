using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetMessage(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId
    ) : IOperation, Expand<GetMessage, GetMessage>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId];
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}";
        public static string OperationId => "get_message";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}";
    }
}