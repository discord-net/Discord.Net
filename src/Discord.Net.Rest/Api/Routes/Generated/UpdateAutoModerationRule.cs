namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateAutoModerationRule(
        Snowflake GuildId,
        Snowflake RuleId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/auto-moderation/rules/{rule_id}";
        public static string OperationId => "update_auto_moderation_rule";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/auto-moderation/rules/{RuleId}";
    }
}