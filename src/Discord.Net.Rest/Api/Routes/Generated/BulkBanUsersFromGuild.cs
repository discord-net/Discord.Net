using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record BulkBanUsersFromGuild(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<BulkBanUsersFromGuild, BulkBanUsersFromGuild>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/bulk-ban";
        public static string OperationId => "bulk_ban_users_from_guild";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/bulk-ban";
    }
}