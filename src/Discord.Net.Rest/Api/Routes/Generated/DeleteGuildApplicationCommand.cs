using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteGuildApplicationCommand(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.GuildId GuildId,
        RouteParameters.CommandId CommandId
    ) : IOperation, Expand<DeleteGuildApplicationCommand, DeleteGuildApplicationCommand>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.GuildId), typeof(RouteParameters.CommandId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, GuildId, CommandId];
    
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands/{command_id}";
        public static string OperationId => "delete_guild_application_command";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands/{CommandId}";
    }
}