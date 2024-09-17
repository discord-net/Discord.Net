using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<IEnumerable<ApplicationCommand>> GetGlobalApplicationCommands(
        [IdHeuristic<IApplication>] ulong applicationId
    ) => new ApiOutRoute<IEnumerable<ApplicationCommand>>(
        nameof(GetGlobalApplicationCommands),
        RequestMethod.Get,
        $"applications/{applicationId}/commands"
    );

    public static IApiInOutRoute<CreateGlobalApplicationCommandParams, ApplicationCommand>
        CreateGlobalApplicationCommand(
            [IdHeuristic<IApplication>] ulong applicationId,
            CreateGlobalApplicationCommandParams body
        ) => new ApiInOutRoute<CreateGlobalApplicationCommandParams, ApplicationCommand>(
        nameof(CreateGlobalApplicationCommand),
        RequestMethod.Post,
        $"applications/{applicationId}/commands",
        body
    );

    public static IApiOutRoute<ApplicationCommand> GetGlobalApplicationCommand(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IApplicationCommand>] ulong commandId
    ) => new ApiOutRoute<ApplicationCommand>(
        nameof(GetGlobalApplicationCommand),
        RequestMethod.Get,
        $"applications/{applicationId}/commands/{commandId}"
    );

    public static IApiInOutRoute<ModifyGlobalApplicationCommandParams, ApplicationCommand>
        ModifyGlobalApplicationCommand(
            [IdHeuristic<IApplication>] ulong applicationId,
            [IdHeuristic<IApplicationCommand>] ulong commandId,
            ModifyGlobalApplicationCommandParams body
        ) => new ApiInOutRoute<ModifyGlobalApplicationCommandParams, ApplicationCommand>(
        nameof(ModifyGlobalApplicationCommand),
        RequestMethod.Patch,
        $"applications/{applicationId}/commands/{commandId}",
        body
    );

    public static IApiRoute DeleteGlobalApplicationCommand(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IApplicationCommand>] ulong commandId
    ) => new ApiRoute(
        nameof(DeleteGlobalApplicationCommand),
        RequestMethod.Delete,
        $"applications/{applicationId}/commands/{commandId}"
    );

    public static IApiInOutRoute<IEnumerable<ApplicationCommand>, IEnumerable<ApplicationCommand>>
        BulkOverwriteGlobalApplicationCommands(
            [IdHeuristic<IApplication>] ulong applicationId,
            IEnumerable<ApplicationCommand> commands
        ) => new ApiInOutRoute<IEnumerable<ApplicationCommand>, IEnumerable<ApplicationCommand>>(
        nameof(BulkOverwriteGlobalApplicationCommands),
        RequestMethod.Put,
        $"applications/{applicationId}/commands",
        commands
    );

    public static IApiOutRoute<IEnumerable<ApplicationCommand>> GetGuildApplicationCommands(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId
    ) => new ApiOutRoute<IEnumerable<ApplicationCommand>>(
        nameof(GetGuildApplicationCommands),
        RequestMethod.Get,
        $"applications/{applicationId}/guilds/{guildId}/commands",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<CreateGuildApplicationCommandParams, ApplicationCommand> CreateGuildApplicationCommand(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId,
        CreateGuildApplicationCommandParams body
    ) => new ApiInOutRoute<CreateGuildApplicationCommandParams, ApplicationCommand>(
        nameof(CreateGuildApplicationCommand),
        RequestMethod.Post,
        $"applications/{applicationId}/guilds/{guildId}/commands",
        body,
        bucket: (ScopeType.Guild, guildId)
    );

    public static IApiOutRoute<ApplicationCommand> GetGuildApplicationCommand(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IApplicationCommand>] ulong commandId
    ) => new ApiOutRoute<ApplicationCommand>(
        nameof(GetGuildApplicationCommand),
        RequestMethod.Get,
        $"applications/{applicationId}/guilds/{guildId}/commands/{commandId}",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<ModifyGuildApplicationCommandParams, ApplicationCommand> ModifyGuildApplicationCommand(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IApplicationCommand>] ulong commandId,
        ModifyGuildApplicationCommandParams body
    ) => new ApiInOutRoute<ModifyGuildApplicationCommandParams, ApplicationCommand>(
        nameof(ModifyGuildApplicationCommand),
        RequestMethod.Patch,
        $"applications/{applicationId}/guilds/{guildId}/commands/{commandId}",
        body,
        bucket: (ScopeType.Guild, guildId)
    );

    public static IApiRoute DeleteGuildApplicationCommand(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IApplicationCommand>] ulong commandId
    ) => new ApiRoute(
        nameof(DeleteGuildApplicationCommand),
        RequestMethod.Delete,
        $"applications/{applicationId}/guilds/{guildId}/commands/{commandId}",
        bucket: (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<IEnumerable<ApplicationCommand>, IEnumerable<ApplicationCommand>>
        BulkOverwriteGuildApplicationCommands(
            [IdHeuristic<IApplication>] ulong applicationId,
            [IdHeuristic<IGuild>] ulong guildId,
            IEnumerable<ApplicationCommand> commands
        ) => new ApiInOutRoute<IEnumerable<ApplicationCommand>, IEnumerable<ApplicationCommand>>(
        nameof(BulkOverwriteGlobalApplicationCommands),
        RequestMethod.Put,
        $"applications/{applicationId}/guilds/{guildId}/commands",
        commands,
        bucket: (ScopeType.Guild, guildId)
    );

    public static IApiOutRoute<IEnumerable<ApplicationCommandPermissions>> GetGuildApplicationCommandPermissions(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId
    ) => new ApiOutRoute<IEnumerable<ApplicationCommandPermissions>>(
        nameof(GetGuildApplicationCommandPermissions),
        RequestMethod.Get,
        $"applications/{applicationId}/guilds/{guildId}/commands/permissions",
        (ScopeType.Guild, guildId)
    );

    public static IApiOutRoute<ApplicationCommandPermissions> GetApplicationCommandPermissions(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IApplicationCommand>] ulong commandId
    ) => new ApiOutRoute<ApplicationCommandPermissions>(
        nameof(GetGuildApplicationCommandPermissions),
        RequestMethod.Get,
        $"applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions",
        (ScopeType.Guild, guildId)
    );

    public static IApiInOutRoute<
        ModifyApplicationCommandPermissionsParams,
        Discord.Models.Json.ApplicationCommandPermissions
    > ModifyApplicationCommandPermissions(
        [IdHeuristic<IApplication>] ulong applicationId,
        [IdHeuristic<IGuild>] ulong guildId,
        [IdHeuristic<IApplicationCommand>] ulong commandId,
        ModifyApplicationCommandPermissionsParams body
    ) => new ApiInOutRoute<
        ModifyApplicationCommandPermissionsParams,
        Discord.Models.Json.ApplicationCommandPermissions
    >(
        nameof(ModifyApplicationCommandPermissions),
        RequestMethod.Put,
        $"applications/{applicationId}/guilds/{guildId}/commands/{commandId}/permissions",
        body,
        bucket: (ScopeType.Guild, guildId)
    );
}