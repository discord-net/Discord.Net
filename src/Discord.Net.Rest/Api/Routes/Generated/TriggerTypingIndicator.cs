namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record TriggerTypingIndicator(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/typing";
        public static string OperationId => "trigger_typing_indicator";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/typing";
    }
}