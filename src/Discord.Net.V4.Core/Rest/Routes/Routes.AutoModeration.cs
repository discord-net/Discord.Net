using Discord.API;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<AutoModerationRule> GetAutoModerationRule(ulong guildId, ulong ruleId)
        => new(nameof(GetAutoModerationRule),
            RequestMethod.Get,
            $"guilds/{guildId}/auto-moderation/rule/{ruleId}",
            (ScopeType.Guild, guildId));

    public static ApiRoute<AutoModerationRule[]> GetAutoModerationRules(ulong guildId)
        => new(nameof(GetAutoModerationRules),
                       RequestMethod.Get,
                       $"guilds/{guildId}/auto-moderation/rules",
                       (ScopeType.Guild, guildId));
}
