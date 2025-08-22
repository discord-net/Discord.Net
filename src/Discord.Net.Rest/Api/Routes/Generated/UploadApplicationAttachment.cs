namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UploadApplicationAttachment(
        Snowflake ApplicationId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/attachment";
        public static string OperationId => "upload_application_attachment";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/attachment";
    }
}