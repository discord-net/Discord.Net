namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildIntegration(
        Snowflake GuildId,
        Snowflake IntegrationId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/integrations/{integration_id}";
        public static string OperationId => "delete_guild_integration";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/integrations/{IntegrationId}";
    }
}