namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record BulkSetApplicationCommands(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/commands";
        public static string OperationId => "bulk_set_application_commands";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands";
    }
}