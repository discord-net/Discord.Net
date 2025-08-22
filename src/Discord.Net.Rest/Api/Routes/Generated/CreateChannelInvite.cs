namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateChannelInvite(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/invites";
        public static string OperationId => "create_channel_invite";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/invites";
    }
}