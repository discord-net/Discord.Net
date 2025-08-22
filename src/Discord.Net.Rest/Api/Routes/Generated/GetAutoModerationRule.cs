namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetAutoModerationRule(
        RouteParameters.GuildId GuildId,
        RouteParameters.RuleId RuleId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.RuleId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, RuleId];
    
        public static string Path => @"/guilds/{guild_id}/auto-moderation/rules/{rule_id}";
        public static string OperationId => "get_auto_moderation_rule";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/auto-moderation/rules/{RuleId}";
    }
}