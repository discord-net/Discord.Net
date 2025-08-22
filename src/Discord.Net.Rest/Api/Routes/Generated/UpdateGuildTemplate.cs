namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildTemplate(
        Snowflake GuildId,
        string Code
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/templates/{code}";
        public static string OperationId => "update_guild_template";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates/{Code}";
    }
}