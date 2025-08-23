using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListGuildVoiceRegions(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<ListGuildVoiceRegions, ListGuildVoiceRegions>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/regions";
        public static string OperationId => "list_guild_voice_regions";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/regions";
    }
}