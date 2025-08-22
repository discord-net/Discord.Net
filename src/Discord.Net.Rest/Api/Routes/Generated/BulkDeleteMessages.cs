namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record BulkDeleteMessages(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/bulk-delete";
        public static string OperationId => "bulk_delete_messages";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/bulk-delete";
    }
}