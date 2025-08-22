namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateApplicationCommand(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/commands";
        public static string OperationId => "create_application_command";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands";
    }
}