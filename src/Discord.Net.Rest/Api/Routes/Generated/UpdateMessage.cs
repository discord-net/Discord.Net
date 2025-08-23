using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateMessage(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId
    ) : IOperation, Expand<UpdateMessage, UpdateMessage>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId];
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}";
        public static string OperationId => "update_message";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}";
    }
}