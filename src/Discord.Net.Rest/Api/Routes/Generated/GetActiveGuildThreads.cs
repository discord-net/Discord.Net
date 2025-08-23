using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetActiveGuildThreads(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<GetActiveGuildThreads, GetActiveGuildThreads>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/threads/active";
        public static string OperationId => "get_active_guild_threads";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/threads/active";
    }
}