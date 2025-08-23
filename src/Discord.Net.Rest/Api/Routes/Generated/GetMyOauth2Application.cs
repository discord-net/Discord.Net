using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetMyOauth2Application() : IOperation, Expand<GetMyOauth2Application, GetMyOauth2Application>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetMyOauth2Application Instance = new();
        public static string Path => @"/oauth2/applications/@me";
        public static string OperationId => "get_my_oauth2_application";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/oauth2/applications/@me";
    }
}