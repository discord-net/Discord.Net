namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateMessage(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages";
        public static string OperationId => "create_message";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages";
    }
}