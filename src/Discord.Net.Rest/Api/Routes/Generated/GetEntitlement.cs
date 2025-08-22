namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetEntitlement(
        Snowflake ApplicationId,
        Snowflake EntitlementId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/entitlements/{entitlement_id}";
        public static string OperationId => "get_entitlement";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements/{EntitlementId}";
    }
}