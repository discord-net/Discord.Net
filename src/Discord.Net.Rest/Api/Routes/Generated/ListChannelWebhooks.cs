namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListChannelWebhooks(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/webhooks";
        public static string OperationId => "list_channel_webhooks";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/webhooks";
    }
}