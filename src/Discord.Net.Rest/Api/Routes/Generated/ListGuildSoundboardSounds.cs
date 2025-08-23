using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListGuildSoundboardSounds(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<ListGuildSoundboardSounds, ListGuildSoundboardSounds>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/soundboard-sounds";
        public static string OperationId => "list_guild_soundboard_sounds";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/soundboard-sounds";
    }
}