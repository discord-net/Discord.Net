namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UnbanUserFromGuild(
        Snowflake GuildId,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/bans/{user_id}";
        public static string OperationId => "unban_user_from_guild";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/bans/{UserId}";
    }
}