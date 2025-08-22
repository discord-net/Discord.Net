namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetApplicationEmoji(
        Snowflake ApplicationId,
        Snowflake EmojiId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/emojis/{emoji_id}";
        public static string OperationId => "get_application_emoji";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/emojis/{EmojiId}";
    }
}