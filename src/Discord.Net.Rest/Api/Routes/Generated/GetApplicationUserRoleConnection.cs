namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetApplicationUserRoleConnection(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/users/@me/applications/{application_id}/role-connection";
        public static string OperationId => "get_application_user_role_connection";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/applications/{ApplicationId}/role-connection";
    }
}