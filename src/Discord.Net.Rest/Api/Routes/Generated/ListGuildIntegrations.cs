namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildIntegrations(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/integrations";
        public static string OperationId => "list_guild_integrations";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/integrations";
    }
}