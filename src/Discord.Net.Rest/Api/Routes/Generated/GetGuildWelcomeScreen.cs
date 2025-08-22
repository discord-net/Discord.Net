namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildWelcomeScreen(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/welcome-screen";
        public static string OperationId => "get_guild_welcome_screen";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/welcome-screen";
    }
}