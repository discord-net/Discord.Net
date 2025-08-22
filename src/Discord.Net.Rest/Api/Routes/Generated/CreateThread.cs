namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateThread(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/threads";
        public static string OperationId => "create_thread";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/threads";
    }
}