using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteGuildMemberRole(
        RouteParameters.GuildId GuildId,
        RouteParameters.UserId UserId,
        RouteParameters.RoleId RoleId
    ) : IOperation, Expand<DeleteGuildMemberRole, DeleteGuildMemberRole>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.UserId), typeof(RouteParameters.RoleId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, UserId, RoleId];
    
        public static string Path => @"/guilds/{guild_id}/members/{user_id}/roles/{role_id}";
        public static string OperationId => "delete_guild_member_role";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/{UserId}/roles/{RoleId}";
    }
}