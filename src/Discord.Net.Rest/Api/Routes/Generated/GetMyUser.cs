using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetMyUser() : IOperation, Expand<GetMyUser, GetMyUser>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetMyUser Instance = new();
        public static string Path => @"/users/@me";
        public static string OperationId => "get_my_user";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me";
    }
}