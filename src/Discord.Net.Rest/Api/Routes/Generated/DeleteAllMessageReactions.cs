namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteAllMessageReactions(
        Snowflake ChannelId,
        Snowflake MessageId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions";
        public static string OperationId => "delete_all_message_reactions";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions";
    }
}