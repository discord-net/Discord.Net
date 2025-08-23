using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetMyApplication() : IOperation, Expand<GetMyApplication, GetMyApplication>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetMyApplication Instance = new();
        public static string Path => @"/applications/@me";
        public static string OperationId => "get_my_application";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/@me";
    }
}