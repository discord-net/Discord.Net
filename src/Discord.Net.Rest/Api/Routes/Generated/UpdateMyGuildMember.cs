namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateMyGuildMember(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/members/@me";
        public static string OperationId => "update_my_guild_member";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/@me";
    }
}