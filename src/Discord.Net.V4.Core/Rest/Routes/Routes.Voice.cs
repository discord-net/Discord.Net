namespace Discord.Rest;

public static partial class Routes
{
    public static readonly IApiOutRoute<VoiceRegion[]> ListVoiceRegions
        = new ApiOutRoute<VoiceRegion[]>(nameof(ListVoiceRegions), RequestMethod.Get, "voice/regions");
}
