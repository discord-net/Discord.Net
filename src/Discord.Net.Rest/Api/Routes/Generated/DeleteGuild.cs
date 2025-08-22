namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuild(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}";
        public static string OperationId => "delete_guild";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}";
    }
}