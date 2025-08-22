namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record LeaveThread(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/thread-members/@me";
        public static string OperationId => "leave_thread";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/thread-members/@me";
    }
}