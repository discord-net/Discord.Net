using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetMyGuildMember(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<GetMyGuildMember, GetMyGuildMember>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/users/@me/guilds/{guild_id}/member";
        public static string OperationId => "get_my_guild_member";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/guilds/{GuildId}/member";
    }
}