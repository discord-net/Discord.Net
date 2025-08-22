namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateThreadFromMessage(
        Snowflake ChannelId,
        Snowflake MessageId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/threads";
        public static string OperationId => "create_thread_from_message";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/threads";
    }
}