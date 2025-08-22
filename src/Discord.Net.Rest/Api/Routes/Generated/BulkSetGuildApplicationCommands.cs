namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record BulkSetGuildApplicationCommands(
        Snowflake ApplicationId,
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands";
        public static string OperationId => "bulk_set_guild_application_commands";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands";
    }
}