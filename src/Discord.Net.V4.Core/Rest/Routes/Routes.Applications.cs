using Discord.API;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<Application> GetCurrentApplication
        => new(nameof(GetCurrentApplication), RequestMethod.Get, "applications/@me");

    public static ApiBodyRoute<ModifyCurrentApplicationParams, Application> ModifyCurrentApplication(ModifyCurrentApplicationParams body)
        => new(nameof(ModifyCurrentApplication),
            RequestMethod.Patch,
            "applications/@me",
            body,
            ContentType.JsonBody);

    public static ApiRoute<ApplicationRoleConnectionMetadata> GetApplicationRoleConnectionMetadata(ulong applicationId, ulong roleId)
        => new(nameof(GetApplicationRoleConnectionMetadata),
            RequestMethod.Get,
            $"applications/{applicationId}/role-connections/{roleId}/metadata");

    public static ApiBodyRoute<ApplicationRoleConnectionMetadata, ApplicationRoleConnectionMetadata> ModifyApplicationRoleConnectionMetadata(ulong applicationId, ulong roleId, ApplicationRoleConnectionMetadata body)
        => new(nameof(GetApplicationRoleConnectionMetadata),
            RequestMethod.Get,
            $"applications/{applicationId}/role-connections/{roleId}/metadata",
            body,
            ContentType.JsonBody);
}
