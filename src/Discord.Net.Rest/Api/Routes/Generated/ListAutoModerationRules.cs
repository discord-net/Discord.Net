namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListAutoModerationRules(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/auto-moderation/rules";
        public static string OperationId => "list_auto_moderation_rules";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/auto-moderation/rules";
    }
}