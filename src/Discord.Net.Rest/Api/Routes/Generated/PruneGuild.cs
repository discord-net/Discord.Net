using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record PruneGuild(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<PruneGuild, PruneGuild>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/prune";
        public static string OperationId => "prune_guild";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/prune";
    }
}