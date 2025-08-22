namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateApplicationEmoji(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/emojis";
        public static string OperationId => "create_application_emoji";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/emojis";
    }
}