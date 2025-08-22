namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildInvites(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/invites";
        public static string OperationId => "list_guild_invites";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/invites";
    }
}