namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record AddGuildMemberRole(
        Snowflake GuildId,
        Snowflake UserId,
        Snowflake RoleId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/members/{user_id}/roles/{role_id}";
        public static string OperationId => "add_guild_member_role";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/{UserId}/roles/{RoleId}";
    }
}