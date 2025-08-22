namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetMyGuildMember(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/users/@me/guilds/{guild_id}/member";
        public static string OperationId => "get_my_guild_member";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/guilds/{GuildId}/member";
    }
}