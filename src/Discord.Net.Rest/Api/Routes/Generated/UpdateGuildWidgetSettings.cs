namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildWidgetSettings(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/widget";
        public static string OperationId => "update_guild_widget_settings";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/widget";
    }
}