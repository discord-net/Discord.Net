namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetApplicationCommand(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.CommandId CommandId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.CommandId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, CommandId];
    
        public static string Path => @"/applications/{application_id}/commands/{command_id}";
        public static string OperationId => "get_application_command";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands/{CommandId}";
    }
}