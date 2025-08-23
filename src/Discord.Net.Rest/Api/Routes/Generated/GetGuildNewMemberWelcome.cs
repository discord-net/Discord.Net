using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildNewMemberWelcome(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<GetGuildNewMemberWelcome, GetGuildNewMemberWelcome>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/new-member-welcome";
        public static string OperationId => "get_guild_new_member_welcome";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/new-member-welcome";
    }
}