using Discord.Models.Json;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly ApiRoute<VoiceRegion[]> ListVoiceRegions
        = new(nameof(ListVoiceRegions), RequestMethod.Get, "voice/regions");
}
