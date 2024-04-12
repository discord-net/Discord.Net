using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class CreatePollParams
{
    [JsonProperty("question")]
    public PollMedia Question { get; set; }

    [JsonProperty("answers")]
    public PollAnswer[] Answers { get; set; }

    [JsonProperty("duration")]
    public uint Duration { get; set; }

    [JsonProperty("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    [JsonProperty("layout_type")]
    public Optional<PollLayout> LayoutType { get; set; }
}
