namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SetGuildMfaLevel(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/mfa";
        public static string OperationId => "set_guild_mfa_level";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/mfa";
    }
}