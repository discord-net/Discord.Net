using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateGuildEmoji(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<CreateGuildEmoji, CreateGuildEmoji>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/emojis";
        public static string OperationId => "create_guild_emoji";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/emojis";
    }
}