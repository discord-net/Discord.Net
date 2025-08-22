namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListApplicationEmojis(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/emojis";
        public static string OperationId => "list_application_emojis";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/emojis";
    }
}