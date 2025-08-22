namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record AddGuildMember(
        Snowflake GuildId,
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/members/{user_id}";
        public static string OperationId => "add_guild_member";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/{UserId}";
    }
}