namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildRole(
        Snowflake GuildId,
        Snowflake RoleId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/roles/{role_id}";
        public static string OperationId => "get_guild_role";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles/{RoleId}";
    }
}