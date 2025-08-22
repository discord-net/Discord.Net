namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record AddMyMessageReaction(
        Snowflake ChannelId,
        Snowflake MessageId,
        string EmojiName
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}/@me";
        public static string OperationId => "add_my_message_reaction";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}/@me";
    }
}