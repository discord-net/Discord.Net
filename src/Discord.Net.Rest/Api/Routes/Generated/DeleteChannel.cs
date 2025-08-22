namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteChannel(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}";
        public static string OperationId => "delete_channel";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}";
    }
}