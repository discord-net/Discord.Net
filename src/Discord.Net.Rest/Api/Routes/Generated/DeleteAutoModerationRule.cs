using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteAutoModerationRule(
        RouteParameters.GuildId GuildId,
        RouteParameters.RuleId RuleId
    ) : IOperation, Expand<DeleteAutoModerationRule, DeleteAutoModerationRule>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.RuleId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, RuleId];
    
        public static string Path => @"/guilds/{guild_id}/auto-moderation/rules/{rule_id}";
        public static string OperationId => "delete_auto_moderation_rule";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/auto-moderation/rules/{RuleId}";
    }
}