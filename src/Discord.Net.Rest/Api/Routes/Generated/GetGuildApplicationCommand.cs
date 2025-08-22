namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildApplicationCommand(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.GuildId GuildId,
        RouteParameters.CommandId CommandId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.GuildId), typeof(RouteParameters.CommandId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, GuildId, CommandId];
    
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands/{command_id}";
        public static string OperationId => "get_guild_application_command";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands/{CommandId}";
    }
}