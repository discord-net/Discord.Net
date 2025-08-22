namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuild(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}";
        public static string OperationId => "update_guild";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}";
    }
}