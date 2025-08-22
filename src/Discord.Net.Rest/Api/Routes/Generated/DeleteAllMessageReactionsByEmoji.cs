namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteAllMessageReactionsByEmoji(
        Snowflake ChannelId,
        Snowflake MessageId,
        string EmojiName
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}";
        public static string OperationId => "delete_all_message_reactions_by_emoji";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}";
    }
}