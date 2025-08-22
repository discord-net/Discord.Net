namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildBan(
        Snowflake GuildId,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/bans/{user_id}";
        public static string OperationId => "get_guild_ban";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/bans/{UserId}";
    }
}