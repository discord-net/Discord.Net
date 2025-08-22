namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildSoundboardSound(
        RouteParameters.GuildId GuildId,
        RouteParameters.SoundId SoundId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.SoundId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, SoundId];
    
        public static string Path => @"/guilds/{guild_id}/soundboard-sounds/{sound_id}";
        public static string OperationId => "update_guild_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/soundboard-sounds/{SoundId}";
    }
}