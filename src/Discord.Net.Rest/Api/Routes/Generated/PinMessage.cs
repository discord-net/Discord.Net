using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record PinMessage(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId
    ) : IOperation, Expand<PinMessage, PinMessage>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId];
    
        public static string Path => @"/channels/{channel_id}/pins/{message_id}";
        public static string OperationId => "pin_message";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/pins/{MessageId}";
    }
}