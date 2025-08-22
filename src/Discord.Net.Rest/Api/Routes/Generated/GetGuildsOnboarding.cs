namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildsOnboarding(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/onboarding";
        public static string OperationId => "get_guilds_onboarding";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/onboarding";
    }
}