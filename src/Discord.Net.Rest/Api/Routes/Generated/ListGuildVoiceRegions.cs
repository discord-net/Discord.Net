namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildVoiceRegions(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/regions";
        public static string OperationId => "list_guild_voice_regions";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/regions";
    }
}