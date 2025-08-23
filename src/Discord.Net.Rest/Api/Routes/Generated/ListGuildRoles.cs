using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListGuildRoles(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<ListGuildRoles, ListGuildRoles>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/roles";
        public static string OperationId => "list_guild_roles";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles";
    }
}