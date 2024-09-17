using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<GuildTemplate> GetGuildTemplate(
        [IdHeuristic<IGuildTemplate>] string templateCode
    ) => new ApiOutRoute<GuildTemplate>(
        nameof(GetGuildTemplate),
        RequestMethod.Get,
        $"guilds/templates/{templateCode}"
    );

    public static IApiInOutRoute<CreateGuildFromTemplateParams, Guild> CreateGuildFromTemplate(
        [IdHeuristic<IGuildTemplate>] string templateCode,
        CreateGuildFromTemplateParams body
    ) => new ApiInOutRoute<CreateGuildFromTemplateParams, Guild>(
        nameof(CreateGuildFromTemplate),
        RequestMethod.Post,
        $"guilds/templates/{templateCode}",
        body
    );

    public static IApiOutRoute<GuildTemplate[]> GetGuildTemplates(
        [IdHeuristic<IGuild>] ulong guildId
    ) => new ApiOutRoute<GuildTemplate[]>(
        nameof(GetGuildTemplates),
        RequestMethod.Get,
        $"guilds/{guildId}/templates",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<CreateTemplateParams, GuildTemplate> CreateGuildTemplate(
        [IdHeuristic<IGuild>] ulong guildId,
        CreateTemplateParams body
    ) => new ApiInOutRoute<CreateTemplateParams, GuildTemplate>(
        nameof(CreateGuildTemplate),
        RequestMethod.Post,
        $"guilds/{guildId}/templates",
        body
    );
    
    public static IApiOutRoute<GuildTemplate> SyncGuildTemplate(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildTemplate>] string templateCode
    ) => new ApiOutRoute<GuildTemplate>(
        nameof(SyncGuildTemplate),
        RequestMethod.Put,
        $"guilds/{guildId}/templates/{templateCode}",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<ModifyGuildTemplateParams, GuildTemplate> ModifyGuildTemplate(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildTemplate>] string templateCode,
        ModifyGuildTemplateParams body
    ) => new ApiInOutRoute<ModifyGuildTemplateParams, GuildTemplate>(
        nameof(ModifyGuildTemplate),
        RequestMethod.Patch,
        $"guilds/{guildId}/templates/{templateCode}",
        body,
        ContentType.JsonBody,
        (ScopeType.Guild, guildId)
    );

    public static IApiOutRoute<GuildTemplate> DeleteGuildTemplate(
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IGuildTemplate>] string templateCode
    ) => new ApiOutRoute<GuildTemplate>(
        nameof(DeleteGuildTemplate),
        RequestMethod.Delete,
        $"guilds/{guildId}/templates/{templateCode}",
        (ScopeType.Guild, guildId)
    );
}