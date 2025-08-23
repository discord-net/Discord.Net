using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildMember(
        RouteParameters.GuildId GuildId,
        RouteParameters.UserId UserId
    ) : IOperation, Expand<GetGuildMember, GetGuildMember>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, UserId];
    
        public static string Path => @"/guilds/{guild_id}/members/{user_id}";
        public static string OperationId => "get_guild_member";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/{UserId}";
    }
}