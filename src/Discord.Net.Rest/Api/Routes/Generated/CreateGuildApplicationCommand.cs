namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildApplicationCommand(
        Snowflake ApplicationId,
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/applications/{application_id}/guilds/{guild_id}/commands";
        public static string OperationId => "create_guild_application_command";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/guilds/{GuildId}/commands";
    }
}