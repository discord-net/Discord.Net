namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildSticker(
        Snowflake GuildId,
        Snowflake StickerId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/stickers/{sticker_id}";
        public static string OperationId => "get_guild_sticker";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/stickers/{StickerId}";
    }
}