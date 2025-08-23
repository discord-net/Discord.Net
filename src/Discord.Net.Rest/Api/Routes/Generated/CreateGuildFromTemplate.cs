using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateGuildFromTemplate(
        RouteParameters.Code Code
    ) : IOperation, Expand<CreateGuildFromTemplate, CreateGuildFromTemplate>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.Code)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [Code];
    
        public static string Path => @"/guilds/templates/{code}";
        public static string OperationId => "create_guild_from_template";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/templates/{Code}";
    }
}