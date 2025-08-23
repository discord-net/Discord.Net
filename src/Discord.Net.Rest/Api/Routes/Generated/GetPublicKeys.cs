using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetPublicKeys() : IOperation, Expand<GetPublicKeys, GetPublicKeys>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetPublicKeys Instance = new();
        public static string Path => @"/oauth2/keys";
        public static string OperationId => "get_public_keys";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/oauth2/keys";
    }
}