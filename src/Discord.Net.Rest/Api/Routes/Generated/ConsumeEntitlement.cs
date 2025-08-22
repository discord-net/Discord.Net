namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ConsumeEntitlement(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.EntitlementId EntitlementId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.EntitlementId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, EntitlementId];
    
        public static string Path => @"/applications/{application_id}/entitlements/{entitlement_id}/consume";
        public static string OperationId => "consume_entitlement";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements/{EntitlementId}/consume";
    }
}