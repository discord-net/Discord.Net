using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateApplicationEmoji(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<CreateApplicationEmoji, CreateApplicationEmoji>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}/emojis";
        public static string OperationId => "create_application_emoji";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/emojis";
    }
}