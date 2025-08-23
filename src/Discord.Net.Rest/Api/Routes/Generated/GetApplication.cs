using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetApplication(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<GetApplication, GetApplication>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}";
        public static string OperationId => "get_application";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}";
    }
}