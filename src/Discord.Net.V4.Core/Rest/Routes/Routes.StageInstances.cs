using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public static partial class Routes
{
    public static IApiInOutRoute<CreateStageInstanceParams, StageInstance> CreateStageInstance(
        CreateStageInstanceParams args) =>
        new ApiInOutRoute<CreateStageInstanceParams, StageInstance>(nameof(CreateStageInstance), RequestMethod.Post,
            "stage-instances", args);

    public static IApiOutRoute<StageInstance> GetStageInstance(ulong channelId) =>
        new ApiOutRoute<StageInstance>(nameof(GetStageInstance), RequestMethod.Get, $"stage-instances/{channelId}");

    public static IApiInOutRoute<ModifyStageInstanceParams, StageInstance> ModifyStageInstance(ulong channelId,
        ModifyStageInstanceParams args) =>
        new ApiInOutRoute<ModifyStageInstanceParams, StageInstance>(nameof(ModifyStageInstance), RequestMethod.Patch,
            $"stage-instance/{channelId}", args);

    public static IApiRoute DeleteStageInstance(ulong channelId) =>
        new ApiRoute(nameof(DeleteStageInstance), RequestMethod.Delete, $"stage-instance/{channelId}");
}
