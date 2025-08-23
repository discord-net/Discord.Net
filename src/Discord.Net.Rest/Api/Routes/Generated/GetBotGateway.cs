using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetBotGateway() : IOperation, Expand<GetBotGateway, GetBotGateway>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetBotGateway Instance = new();
        public static string Path => @"/gateway/bot";
        public static string OperationId => "get_bot_gateway";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/gateway/bot";
    }
}