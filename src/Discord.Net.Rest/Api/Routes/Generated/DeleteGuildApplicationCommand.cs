namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildApplicationCommand(
        Snowflake ApplicationId,
        Snowflake GuildId,
        Snowflake CommandId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands/{command_id}";
        public static string OperationId => "delete_guild_application_command";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands/{CommandId}";
    }
}