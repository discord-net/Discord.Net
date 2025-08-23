using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record TriggerTypingIndicator(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<TriggerTypingIndicator, TriggerTypingIndicator>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/typing";
        public static string OperationId => "trigger_typing_indicator";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/typing";
    }
}