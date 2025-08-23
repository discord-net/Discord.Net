using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildTemplate(
        RouteParameters.Code Code
    ) : IOperation, Expand<GetGuildTemplate, GetGuildTemplate>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.Code)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [Code];
    
        public static string Path => @"/guilds/templates/{code}";
        public static string OperationId => "get_guild_template";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/templates/{Code}";
    }
}