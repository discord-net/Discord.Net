namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetApplicationRoleConnectionsMetadata(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/role-connections/metadata";
        public static string OperationId => "get_application_role_connections_metadata";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/role-connections/metadata";
    }
}