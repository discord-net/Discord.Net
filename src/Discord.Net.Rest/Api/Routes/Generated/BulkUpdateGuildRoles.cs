namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record BulkUpdateGuildRoles(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/roles";
        public static string OperationId => "bulk_update_guild_roles";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles";
    }
}