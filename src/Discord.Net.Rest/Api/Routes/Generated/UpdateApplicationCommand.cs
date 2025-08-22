namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateApplicationCommand(
        Snowflake ApplicationId,
        Snowflake CommandId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/commands/{command_id}";
        public static string OperationId => "update_application_command";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands/{CommandId}";
    }
}