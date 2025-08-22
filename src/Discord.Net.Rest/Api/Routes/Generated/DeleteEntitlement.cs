namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteEntitlement(
        Snowflake ApplicationId,
        Snowflake EntitlementId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/entitlements/{entitlement_id}";
        public static string OperationId => "delete_entitlement";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements/{EntitlementId}";
    }
}