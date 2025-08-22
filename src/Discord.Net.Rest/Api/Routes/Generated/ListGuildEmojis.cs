namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildEmojis(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/emojis";
        public static string OperationId => "list_guild_emojis";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis";
    }
}