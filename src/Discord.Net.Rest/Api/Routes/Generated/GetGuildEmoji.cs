namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildEmoji(
        Snowflake GuildId,
        Snowflake EmojiId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/emojis/{emoji_id}";
        public static string OperationId => "get_guild_emoji";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis/{EmojiId}";
    }
}