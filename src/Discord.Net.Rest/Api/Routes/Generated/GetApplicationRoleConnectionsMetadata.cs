using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetApplicationRoleConnectionsMetadata(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<GetApplicationRoleConnectionsMetadata, GetApplicationRoleConnectionsMetadata>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}/role-connections/metadata";
        public static string OperationId => "get_application_role_connections_metadata";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/role-connections/metadata";
    }
}