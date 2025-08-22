namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateApplicationRoleConnectionsMetadata(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/role-connections/metadata";
        public static string OperationId => "update_application_role_connections_metadata";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/role-connections/metadata";
    }
}