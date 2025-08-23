using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record PollExpire(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId
    ) : IOperation, Expand<PollExpire, PollExpire>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId];
    
        public static string Path => @"/channels/{channel_id}/polls/{message_id}/expire";
        public static string OperationId => "poll_expire";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/polls/{MessageId}/expire";
    }
}