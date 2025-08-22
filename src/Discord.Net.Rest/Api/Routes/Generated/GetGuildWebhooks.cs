namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildWebhooks(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/webhooks";
        public static string OperationId => "get_guild_webhooks";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/webhooks";
    }
}