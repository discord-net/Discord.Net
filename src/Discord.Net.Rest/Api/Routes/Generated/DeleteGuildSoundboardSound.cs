namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildSoundboardSound(
        Snowflake GuildId,
        Snowflake SoundId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/soundboard-sounds/{sound_id}";
        public static string OperationId => "delete_guild_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/soundboard-sounds/{SoundId}";
    }
}