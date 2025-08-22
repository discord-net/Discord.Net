namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildEmoji(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/emojis";
        public static string OperationId => "create_guild_emoji";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis";
    }
}