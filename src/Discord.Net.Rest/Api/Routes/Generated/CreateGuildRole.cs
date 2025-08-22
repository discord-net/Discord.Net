namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildRole(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/roles";
        public static string OperationId => "create_guild_role";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles";
    }
}