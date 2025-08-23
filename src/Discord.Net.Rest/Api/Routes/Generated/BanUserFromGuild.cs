using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record BanUserFromGuild(
        RouteParameters.GuildId GuildId,
        RouteParameters.UserId UserId
    ) : IOperation, Expand<BanUserFromGuild, BanUserFromGuild>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, UserId];
    
        public static string Path => @"/guilds/{guild_id}/bans/{user_id}";
        public static string OperationId => "ban_user_from_guild";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/bans/{UserId}";
    }
}