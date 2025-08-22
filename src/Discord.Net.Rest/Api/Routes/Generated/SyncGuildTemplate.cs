namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SyncGuildTemplate(
        Snowflake GuildId,
        string Code
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/templates/{code}";
        public static string OperationId => "sync_guild_template";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates/{Code}";
    }
}