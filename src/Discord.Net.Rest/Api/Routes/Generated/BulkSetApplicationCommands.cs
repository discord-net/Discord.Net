using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record BulkSetApplicationCommands(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<BulkSetApplicationCommands, BulkSetApplicationCommands>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}/commands";
        public static string OperationId => "bulk_set_application_commands";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands";
    }
}