namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetChannel(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}";
        public static string OperationId => "get_channel";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}";
    }
}