namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record PollExpire(
        Snowflake ChannelId,
        Snowflake MessageId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/polls/{message_id}/expire";
        public static string OperationId => "poll_expire";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/polls/{MessageId}/expire";
    }
}