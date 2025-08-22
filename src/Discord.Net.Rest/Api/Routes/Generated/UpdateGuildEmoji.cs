namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildEmoji(
        Snowflake GuildId,
        Snowflake EmojiId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/emojis/{emoji_id}";
        public static string OperationId => "update_guild_emoji";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis/{EmojiId}";
    }
}