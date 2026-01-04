using Newtonsoft.Json;

namespace Discord.API.Voice;

internal class DaveMLSTransitionParams
{
    [JsonProperty("transition_id")]
    public ushort TransitionId { get; set; }
}
