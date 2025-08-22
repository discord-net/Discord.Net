namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetUser(
        RouteParameters.UserId UserId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [UserId];
    
        public static string Path => @"/users/{user_id}";
        public static string OperationId => "get_user";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/users/{UserId}";
    }
}