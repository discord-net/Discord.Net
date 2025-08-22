namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildTemplate(
        Snowflake GuildId,
        string Code
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/templates/{code}";
        public static string OperationId => "delete_guild_template";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates/{Code}";
    }
}