using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public static partial class Routes
{
    public static ApiRoute<StageInstance> GetStageInstance(ulong channelId)
        => new(nameof(GetStageInstance), RequestMethod.Get, $"stage-instances/{channelId}");

    public static ApiBodyRoute<ModifyStageInstanceParams, StageInstance> ModifyStageInstance(ulong channelId, ModifyStageInstanceParams args)
        => new(nameof(ModifyStageInstance), RequestMethod.Patch, $"stage-instance/{channelId}", args);

    public static BasicApiRoute DeleteStageInstance(ulong channelId)
        => new(nameof(DeleteStageInstance), RequestMethod.Delete, $"stage-instance/{channelId}");
}
