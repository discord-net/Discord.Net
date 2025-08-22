namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildScheduledEvent(
        Snowflake GuildId,
        Snowflake GuildScheduledEventId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/scheduled-events/{guild_scheduled_event_id}";
        public static string OperationId => "delete_guild_scheduled_event";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/scheduled-events/{GuildScheduledEventId}";
    }
}