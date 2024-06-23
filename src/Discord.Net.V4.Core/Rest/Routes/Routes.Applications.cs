using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<Application> GetCurrentApplication
        => new ApiOutRoute<Application>(nameof(GetCurrentApplication), RequestMethod.Get, "applications/@me");

    public static IApiInOutRoute<ModifyCurrentApplicationParams, Application> ModifyCurrentApplication(
        ModifyCurrentApplicationParams body)
        => new ApiInOutRoute<ModifyCurrentApplicationParams, Application>(nameof(ModifyCurrentApplication),
            RequestMethod.Patch,
            "applications/@me",
            body);

    public static IApiOutRoute<ApplicationRoleConnectionMetadata> GetApplicationRoleConnectionMetadata(
        ulong applicationId,
        ulong roleId)
        => new ApiOutRoute<ApplicationRoleConnectionMetadata>(nameof(GetApplicationRoleConnectionMetadata),
            RequestMethod.Get,
            $"applications/{applicationId}/role-connections/{roleId}/metadata");

    public static IApiInOutRoute<ApplicationRoleConnectionMetadata, ApplicationRoleConnectionMetadata>
        ModifyApplicationRoleConnectionMetadata(ulong applicationId, ulong roleId,
            ApplicationRoleConnectionMetadata body)
        => new ApiInOutRoute<ApplicationRoleConnectionMetadata, ApplicationRoleConnectionMetadata>(
            nameof(GetApplicationRoleConnectionMetadata),
            RequestMethod.Get,
            $"applications/{applicationId}/role-connections/{roleId}/metadata",
            body);
}
