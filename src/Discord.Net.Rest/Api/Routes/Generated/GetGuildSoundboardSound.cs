using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildSoundboardSound(
        RouteParameters.GuildId GuildId,
        RouteParameters.SoundId SoundId
    ) : IOperation, Expand<GetGuildSoundboardSound, GetGuildSoundboardSound>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.SoundId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, SoundId];
    
        public static string Path => @"/guilds/{guild_id}/soundboard-sounds/{sound_id}";
        public static string OperationId => "get_guild_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/soundboard-sounds/{SoundId}";
    }
}