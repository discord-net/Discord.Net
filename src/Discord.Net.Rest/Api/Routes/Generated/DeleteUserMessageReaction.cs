namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteUserMessageReaction(
        Snowflake ChannelId,
        Snowflake MessageId,
        string EmojiName,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}/{user_id}";
        public static string OperationId => "delete_user_message_reaction";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}/{UserId}";
    }
}