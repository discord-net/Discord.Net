namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record AddThreadMember(
        Snowflake ChannelId,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/thread-members/{user_id}";
        public static string OperationId => "add_thread_member";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/thread-members/{UserId}";
    }
}