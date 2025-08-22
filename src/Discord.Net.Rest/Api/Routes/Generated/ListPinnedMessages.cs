namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListPinnedMessages(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/pins";
        public static string OperationId => "list_pinned_messages";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/pins";
    }
}