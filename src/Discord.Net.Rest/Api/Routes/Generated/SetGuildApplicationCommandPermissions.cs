namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SetGuildApplicationCommandPermissions(
        Snowflake ApplicationId,
        Snowflake GuildId,
        Snowflake CommandId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands/{command_id}/permissions";
        public static string OperationId => "set_guild_application_command_permissions";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands/{CommandId}/permissions";
    }
}