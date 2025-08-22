namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateEntitlement(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/entitlements";
        public static string OperationId => "create_entitlement";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements";
    }
}