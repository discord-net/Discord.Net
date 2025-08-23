using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record AddMyMessageReaction(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId,
        RouteParameters.EmojiName EmojiName
    ) : IOperation, Expand<AddMyMessageReaction, AddMyMessageReaction>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId), typeof(RouteParameters.EmojiName)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId, EmojiName];
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}/@me";
        public static string OperationId => "add_my_message_reaction";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}/@me";
    }
}