using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildScheduledEvent(
        RouteParameters.GuildId GuildId,
        RouteParameters.GuildScheduledEventId GuildScheduledEventId
    ) : IOperation, Expand<GetGuildScheduledEvent, GetGuildScheduledEvent>
    {
        public Optional<bool> WithUserCount { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.GuildScheduledEventId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, GuildScheduledEventId];
    
        public static string Path => @"/guilds/{guild_id}/scheduled-events/{guild_scheduled_event_id}";
        public static string OperationId => "get_guild_scheduled_event";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/scheduled-events/{GuildScheduledEventId}{QueryStrings.Build(("with_user_count", WithUserCount.ToNullable()))}";
    }
}