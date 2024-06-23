using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<GuildTemplate> GetGuildTemplate(string templateCode) =>
        new ApiOutRoute<GuildTemplate>(nameof(GetGuildTemplate), RequestMethod.Get, $"guilds/templates/{templateCode}");

    public static IApiInOutRoute<CreateGuildFromTemplateParams, Guild> CreateGuildFromTemplate(string templateCode,
        CreateGuildFromTemplateParams body) =>
        new ApiInOutRoute<CreateGuildFromTemplateParams, Guild>(nameof(CreateGuildFromTemplate), RequestMethod.Post,
            $"guilds/templates/{templateCode}", body);

    public static IApiOutRoute<GuildTemplate[]> GetGuildTemplates(ulong guildId) =>
        new ApiOutRoute<GuildTemplate[]>(nameof(GetGuildTemplates), RequestMethod.Get, $"guilds/{guildId}/templates",
            (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildTemplate> SyncGuildTemplate(ulong guildId, string templateCode) =>
        new ApiOutRoute<GuildTemplate>(nameof(SyncGuildTemplate), RequestMethod.Put,
            $"guilds/{guildId}/templates/{templateCode}", (ScopeType.Guild, guildId));

    public static IApiInOutRoute<ModifyGuildTemplateParams, GuildTemplate> ModifyGuildTemplate(ulong guildId,
        string templateCode, ModifyGuildTemplateParams body) =>
        new ApiInOutRoute<ModifyGuildTemplateParams, GuildTemplate>(nameof(ModifyGuildTemplate), RequestMethod.Patch,
            $"guilds/{guildId}/templates/{templateCode}", body, ContentType.JsonBody, (ScopeType.Guild, guildId));

    public static IApiOutRoute<GuildTemplate> DeleteGuildTemplate(ulong guildId, string templateCode) =>
        new ApiOutRoute<GuildTemplate>(nameof(DeleteGuildTemplate), RequestMethod.Delete,
            $"guilds/{guildId}/templates/{templateCode}", (ScopeType.Guild, guildId));
}
