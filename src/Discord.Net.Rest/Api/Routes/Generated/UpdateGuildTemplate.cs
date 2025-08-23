using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateGuildTemplate(
        RouteParameters.GuildId GuildId,
        RouteParameters.Code Code
    ) : IOperation, Expand<UpdateGuildTemplate, UpdateGuildTemplate>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.Code)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, Code];
    
        public static string Path => @"/guilds/{guild_id}/templates/{code}";
        public static string OperationId => "update_guild_template";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates/{Code}";
    }
}