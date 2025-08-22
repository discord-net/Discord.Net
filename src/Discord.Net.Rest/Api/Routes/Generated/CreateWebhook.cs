namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateWebhook(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/webhooks";
        public static string OperationId => "create_webhook";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/webhooks";
    }
}