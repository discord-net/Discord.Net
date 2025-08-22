namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CrosspostMessage(
        Snowflake ChannelId,
        Snowflake MessageId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/crosspost";
        public static string OperationId => "crosspost_message";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/crosspost";
    }
}