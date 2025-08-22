namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetVoiceState(
        Snowflake GuildId,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/voice-states/{user_id}";
        public static string OperationId => "get_voice_state";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/voice-states/{UserId}";
    }
}