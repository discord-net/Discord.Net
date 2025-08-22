namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildApplicationCommands(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public Optional<bool> WithLocalizations { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, GuildId];
    
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands";
        public static string OperationId => "list_guild_application_commands";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands{QueryStrings.Build(("with_localizations", WithLocalizations.ToNullable()))}";
    }
}