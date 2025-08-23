using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateMyApplication() : IOperation, Expand<UpdateMyApplication, UpdateMyApplication>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly UpdateMyApplication Instance = new();
        public static string Path => @"/applications/@me";
        public static string OperationId => "update_my_application";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/@me";
    }
}