namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildSticker(
        Snowflake GuildId,
        Snowflake StickerId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/stickers/{sticker_id}";
        public static string OperationId => "update_guild_sticker";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/stickers/{StickerId}";
    }
}