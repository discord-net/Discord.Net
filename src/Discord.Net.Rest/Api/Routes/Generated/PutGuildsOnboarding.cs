namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record PutGuildsOnboarding(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/onboarding";
        public static string OperationId => "put_guilds_onboarding";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/onboarding";
    }
}