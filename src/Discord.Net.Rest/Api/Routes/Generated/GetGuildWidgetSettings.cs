namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildWidgetSettings(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/widget";
        public static string OperationId => "get_guild_widget_settings";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/widget";
    }
}