namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListChannelInvites(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/invites";
        public static string OperationId => "list_channel_invites";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/invites";
    }
}