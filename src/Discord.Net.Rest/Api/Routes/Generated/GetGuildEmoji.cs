using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildEmoji(
        RouteParameters.GuildId GuildId,
        RouteParameters.EmojiId EmojiId
    ) : IOperation, Expand<GetGuildEmoji, GetGuildEmoji>
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