using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteApplicationCommand(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.CommandId CommandId
    ) : IOperation, Expand<DeleteApplicationCommand, DeleteApplicationCommand>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.CommandId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, CommandId];
    
        public static string Path => @"/applications/{application_id}/commands/{command_id}";
        public static string OperationId => "delete_application_command";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands/{CommandId}";
    }
}