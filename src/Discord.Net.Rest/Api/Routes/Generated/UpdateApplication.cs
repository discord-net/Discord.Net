namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateApplication(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}";
        public static string OperationId => "update_application";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}";
    }
}