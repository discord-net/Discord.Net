using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListApplicationEmojis(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<ListApplicationEmojis, ListApplicationEmojis>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}/emojis";
        public static string OperationId => "list_application_emojis";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/emojis";
    }
}