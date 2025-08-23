using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListGuildApplicationCommandPermissions(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<ListGuildApplicationCommandPermissions, ListGuildApplicationCommandPermissions>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, GuildId];
    
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands/permissions";
        public static string OperationId => "list_guild_application_command_permissions";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands/permissions";
    }
}