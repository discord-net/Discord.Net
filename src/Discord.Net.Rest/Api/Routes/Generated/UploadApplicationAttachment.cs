using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UploadApplicationAttachment(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<UploadApplicationAttachment, UploadApplicationAttachment>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}/attachment";
        public static string OperationId => "upload_application_attachment";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/attachment";
    }
}