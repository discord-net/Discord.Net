namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildTemplate(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/templates";
        public static string OperationId => "create_guild_template";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates";
    }
}