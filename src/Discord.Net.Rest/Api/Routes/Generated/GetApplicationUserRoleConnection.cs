namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetApplicationUserRoleConnection(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/users/@me/applications/{application_id}/role-connection";
        public static string OperationId => "get_application_user_role_connection";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/applications/{ApplicationId}/role-connection";
    }
}