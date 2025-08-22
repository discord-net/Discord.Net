namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetSelfVoiceState(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/voice-states/@me";
        public static string OperationId => "get_self_voice_state";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/voice-states/@me";
    }
}