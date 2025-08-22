namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildRoles(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/roles";
        public static string OperationId => "list_guild_roles";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles";
    }
}