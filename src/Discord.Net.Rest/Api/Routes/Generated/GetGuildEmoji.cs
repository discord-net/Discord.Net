namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildEmoji(
        RouteParameters.GuildId GuildId,
        RouteParameters.EmojiId EmojiId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.EmojiId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, EmojiId];
    
        public static string Path => @"/guilds/{guild_id}/emojis/{emoji_id}";
        public static string OperationId => "get_guild_emoji";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis/{EmojiId}";
    }
}