namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildApplicationCommandPermissions(
        Snowflake ApplicationId,
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands/permissions";
        public static string OperationId => "list_guild_application_command_permissions";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands/permissions";
    }
}