namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildTemplates(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/templates";
        public static string OperationId => "list_guild_templates";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates";
    }
}