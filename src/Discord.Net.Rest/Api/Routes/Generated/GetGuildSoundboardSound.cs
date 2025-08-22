namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildSoundboardSound(
        Snowflake GuildId,
        Snowflake SoundId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/soundboard-sounds/{sound_id}";
        public static string OperationId => "get_guild_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/soundboard-sounds/{SoundId}";
    }
}