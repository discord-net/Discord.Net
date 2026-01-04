using Newtonsoft.Json;

namespace Discord.API.Voice;

public class DaveMLSTransitionParams
{
    [JsonProperty("transition_id")]
    public ushort TransitionId { get; set; }
}
