namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteAllMessageReactionsByEmoji(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId,
        RouteParameters.EmojiName EmojiName
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId), typeof(RouteParameters.EmojiName)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId, EmojiName];
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}";
        public static string OperationId => "delete_all_message_reactions_by_emoji";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}";
    }
}