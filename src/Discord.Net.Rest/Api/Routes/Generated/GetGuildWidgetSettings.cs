using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildWidgetSettings(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<GetGuildWidgetSettings, GetGuildWidgetSettings>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/widget";
        public static string OperationId => "get_guild_widget_settings";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/widget";
    }
}