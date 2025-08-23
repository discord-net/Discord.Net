using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetMyOauth2Authorization() : IOperation, Expand<GetMyOauth2Authorization, GetMyOauth2Authorization>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetMyOauth2Authorization Instance = new();
        public static string Path => @"/oauth2/@me";
        public static string OperationId => "get_my_oauth2_authorization";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/oauth2/@me";
    }
}