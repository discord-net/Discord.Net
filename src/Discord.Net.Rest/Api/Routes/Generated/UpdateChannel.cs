namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateChannel(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}";
        public static string OperationId => "update_channel";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}";
    }
}