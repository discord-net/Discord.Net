namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ConsumeEntitlement(
        Snowflake ApplicationId,
        Snowflake EntitlementId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/entitlements/{entitlement_id}/consume";
        public static string OperationId => "consume_entitlement";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements/{EntitlementId}/consume";
    }
}