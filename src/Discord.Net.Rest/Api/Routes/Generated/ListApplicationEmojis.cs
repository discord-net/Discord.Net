namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListApplicationEmojis(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation
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