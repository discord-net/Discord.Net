using Discord.API;

namespace Discord.Rest;

public static partial class Routes
{
    public static readonly APIRoute<VoiceRegion[]> ListVoiceRegions
        = new(nameof(ListVoiceRegions), RequestMethod.Get, "voice/regions");
}
