namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildSticker(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/stickers";
        public static string OperationId => "create_guild_sticker";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/stickers";
    }
}