using Discord.API;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<Application> GetCurrentApplication
        => new(nameof(GetCurrentApplication), RequestMethod.Get, "applications/@me");

    public static ApiRoute<ApplicationRoleConnectionMetadata> GetApplicationRoleConnectionMetadata(ulong applicationId, ulong roleId)
        => new(nameof(GetApplicationRoleConnectionMetadata),
            RequestMethod.Get,
            $"applications/{applicationId}/role-connections/{roleId}/metadata");
}
