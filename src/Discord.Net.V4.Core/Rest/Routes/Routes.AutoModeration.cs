using Discord.API;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<AutoModerationRule[]> GetAutoModerationRules(ulong guildId)
        => new(nameof(GetAutoModerationRules),
                       RequestMethod.Get,
                       $"guilds/{guildId}/auto-moderation/rules",
                       (ScopeType.Guild, guildId));

    public static ApiRoute<AutoModerationRule> GetAutoModerationRule(ulong guildId, ulong ruleId)
        => new(nameof(GetAutoModerationRule),
            RequestMethod.Get,
            $"guilds/{guildId}/auto-moderation/rule/{ruleId}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<CreateAutoModRuleParams, AutoModerationRule> CreateAutoModerationRule(ulong guildId, CreateAutoModRuleParams body)
        => new(nameof(CreateAutoModerationRule),
            RequestMethod.Post,
            $"/guilds/{guildId}/auto-moderation/rules",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyAutoModRuleParams, AutoModerationRule> ModifyAutoModerationRule(ulong guildId, ulong ruleId, ModifyAutoModRuleParams body)
        => new(nameof(ModifyAutoModerationRule),
            RequestMethod.Patch,
            $"/guilds/{guildId}/auto-moderation/rules/{ruleId}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static BasicApiRoute DeleteAutoModerationRule(ulong guildId, ulong ruleId)
        => new(nameof(DeleteAutoModerationRule),
            RequestMethod.Delete,
            $"/guilds/{guildId}/auto-moderation/rules/{ruleId}",
            (ScopeType.Guild, guildId));
}
