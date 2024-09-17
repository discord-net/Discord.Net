using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<Application> GetCurrentApplication
        => new ApiOutRoute<Application>(nameof(GetCurrentApplication), RequestMethod.Get, "applications/@me");

    public static IApiInOutRoute<ModifyCurrentApplicationParams, Application> ModifyCurrentApplication(
        ModifyCurrentApplicationParams body
    ) => new ApiInOutRoute<ModifyCurrentApplicationParams, Application>(
        nameof(ModifyCurrentApplication),
        RequestMethod.Patch,
        "applications/@me",
        body
    );

    public static IApiOutRoute<IEnumerable<ApplicationRoleConnectionMetadata>> GetApplicationRoleConnectionMetadata(
        [IdHeuristic<IApplication>] ulong applicationId
    ) => new ApiOutRoute<IEnumerable<ApplicationRoleConnectionMetadata>>(nameof(GetApplicationRoleConnectionMetadata),
        RequestMethod.Get,
        $"applications/{applicationId}/role-connections/metadata"
    );

    public static IApiInOutRoute<
        IEnumerable<ApplicationRoleConnectionMetadata>,
        IEnumerable<ApplicationRoleConnectionMetadata>
    > ModifyApplicationRoleConnectionMetadata(
        [IdHeuristic<IApplication>] ulong applicationId,
        IEnumerable<ApplicationRoleConnectionMetadata> body
    ) => new ApiInOutRoute<
        IEnumerable<ApplicationRoleConnectionMetadata>,
        IEnumerable<ApplicationRoleConnectionMetadata>
    >(
        nameof(ModifyApplicationRoleConnectionMetadata),
        RequestMethod.Put,
        $"applications/{applicationId}/role-connections/metadata",
        body
    );

    
}