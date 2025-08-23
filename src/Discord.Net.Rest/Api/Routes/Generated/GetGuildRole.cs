using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildRole(
        RouteParameters.GuildId GuildId,
        RouteParameters.RoleId RoleId
    ) : IOperation, Expand<GetGuildRole, GetGuildRole>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.RoleId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, RoleId];
    
        public static string Path => @"/guilds/{guild_id}/roles/{role_id}";
        public static string OperationId => "get_guild_role";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles/{RoleId}";
    }
}