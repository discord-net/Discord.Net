using Newtonsoft.Json;
using System;

namespace Discord.API;

internal class Poll
{
    [JsonProperty("question")]
    public PollMedia Question { get; set; }

    [JsonProperty("answers")]
    public PollAnswer[] Answers { get; set; }

    [JsonProperty("expiry")]
    public DateTimeOffset Expiry { get; set; }

    [JsonProperty("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    [JsonProperty("layout_type")]
    public PollLayout LayoutType { get; set; }

    [JsonProperty("results")]
    public Optional<PollResults> PollResults { get; set; }
}
