namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record PinMessage(
        Snowflake ChannelId,
        Snowflake MessageId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/pins/{message_id}";
        public static string OperationId => "pin_message";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/pins/{MessageId}";
    }
}