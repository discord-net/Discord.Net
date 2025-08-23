using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateGuildEmoji(
        RouteParameters.GuildId GuildId,
        RouteParameters.EmojiId EmojiId
    ) : IOperation, Expand<UpdateGuildEmoji, UpdateGuildEmoji>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.EmojiId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, EmojiId];
    
        public static string Path => @"/guilds/{guild_id}/emojis/{emoji_id}";
        public static string OperationId => "update_guild_emoji";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis/{EmojiId}";
    }
}