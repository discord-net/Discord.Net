namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateSelfVoiceState(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/voice-states/@me";
        public static string OperationId => "update_self_voice_state";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/voice-states/@me";
    }
}