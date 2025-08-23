using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record AddGuildMemberRole(
        RouteParameters.GuildId GuildId,
        RouteParameters.UserId UserId,
        RouteParameters.RoleId RoleId
    ) : IOperation, Expand<AddGuildMemberRole, AddGuildMemberRole>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.UserId), typeof(RouteParameters.RoleId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, UserId, RoleId];
    
        public static string Path => @"/guilds/{guild_id}/members/{user_id}/roles/{role_id}";
        public static string OperationId => "add_guild_member_role";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/{UserId}/roles/{RoleId}";
    }
}