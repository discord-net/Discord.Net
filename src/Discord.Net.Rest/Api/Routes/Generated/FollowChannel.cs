namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record FollowChannel(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/followers";
        public static string OperationId => "follow_channel";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/followers";
    }
}