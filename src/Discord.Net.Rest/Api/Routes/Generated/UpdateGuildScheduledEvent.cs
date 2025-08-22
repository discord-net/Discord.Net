namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildScheduledEvent(
        Snowflake GuildId,
        Snowflake GuildScheduledEventId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/scheduled-events/{guild_scheduled_event_id}";
        public static string OperationId => "update_guild_scheduled_event";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/scheduled-events/{GuildScheduledEventId}";
    }
}