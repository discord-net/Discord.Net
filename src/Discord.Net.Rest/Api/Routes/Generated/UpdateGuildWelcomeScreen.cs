namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildWelcomeScreen(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/welcome-screen";
        public static string OperationId => "update_guild_welcome_screen";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/welcome-screen";
    }
}