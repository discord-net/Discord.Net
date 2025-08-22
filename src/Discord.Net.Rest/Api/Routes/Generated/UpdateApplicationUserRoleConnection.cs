namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateApplicationUserRoleConnection(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/users/@me/applications/{application_id}/role-connection";
        public static string OperationId => "update_application_user_role_connection";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/applications/{ApplicationId}/role-connection";
    }
}