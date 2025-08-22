namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record PruneGuild(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/prune";
        public static string OperationId => "prune_guild";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/prune";
    }
}