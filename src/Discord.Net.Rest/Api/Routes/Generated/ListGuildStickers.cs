namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildStickers(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/stickers";
        public static string OperationId => "list_guild_stickers";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/stickers";
    }
}