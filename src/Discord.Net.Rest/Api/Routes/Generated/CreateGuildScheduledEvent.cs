namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildScheduledEvent(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/scheduled-events";
        public static string OperationId => "create_guild_scheduled_event";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/scheduled-events";
    }
}