using Discord.API;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<GuildTemplate> GetGuildTemplate(string templateCode)
        => new(nameof(GetGuildTemplate),
            RequestMethod.Get,
            $"guilds/templates/{templateCode}");

    public static ApiBodyRoute<CreateGuildFromTemplateParams, Guild> CreateGuildFromTemplate(string templateCode, CreateGuildFromTemplateParams body)
        => new(nameof(CreateGuildFromTemplate),
            RequestMethod.Post,
            $"guilds/templates/{templateCode}",
            body);

    public static ApiRoute<GuildTemplate[]> GetGuildTemplates(ulong guildId)
        => new(nameof(GetGuildTemplates),
            RequestMethod.Get,
            $"guilds/{guildId}/templates",
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildTemplate> SyncGuildTemplate(ulong guildId, string templateCode)
        => new(nameof(SyncGuildTemplate),
            RequestMethod.Put,
            $"guilds/{guildId}/templates/{templateCode}",
            (ScopeType.Guild, guildId));

    public static ApiBodyRoute<ModifyGuildTemplateParams, GuildTemplate> ModifyGuildTemplate(ulong guildId, string templateCode, ModifyGuildTemplateParams body)
        => new(nameof(ModifyGuildTemplate),
            RequestMethod.Patch,
            $"guilds/{guildId}/templates/{templateCode}",
            body,
            ContentType.JsonBody,
            (ScopeType.Guild, guildId));

    public static ApiRoute<GuildTemplate> DeleteGuildTemplate(ulong guildId, string templateCode)
        => new(nameof(DeleteGuildTemplate),
            RequestMethod.Delete,
            $"guilds/{guildId}/templates/{templateCode}",
            (ScopeType.Guild, guildId));
}
