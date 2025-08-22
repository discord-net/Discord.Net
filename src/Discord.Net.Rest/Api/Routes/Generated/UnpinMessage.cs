namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UnpinMessage(
        Snowflake ChannelId,
        Snowflake MessageId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/pins/{message_id}";
        public static string OperationId => "unpin_message";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/pins/{MessageId}";
    }
}