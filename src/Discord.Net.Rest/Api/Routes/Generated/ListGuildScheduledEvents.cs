namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildScheduledEvents(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public Optional<bool> WithUserCount { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/scheduled-events";
        public static string OperationId => "list_guild_scheduled_events";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/scheduled-events{QueryStrings.Build(("with_user_count", WithUserCount.ToNullable()))}";
    }
}