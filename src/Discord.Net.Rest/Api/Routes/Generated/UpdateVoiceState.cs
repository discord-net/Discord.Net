namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateVoiceState(
        Snowflake GuildId,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/voice-states/{user_id}";
        public static string OperationId => "update_voice_state";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/voice-states/{UserId}";
    }
}