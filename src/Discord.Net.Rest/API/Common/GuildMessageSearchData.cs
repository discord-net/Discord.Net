using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API;

internal class GuildMessageSearchData
{
    [JsonProperty("doing_deep_historical_index")]
    public bool DoingDeepHistoricalIndex { get; set; }

    [JsonProperty("documents_indexed")]
    public Optional<int> DocumentsIndexed { get; set; }

    [JsonProperty("total_results")]
    public int TotalResults { get; set; }

    [JsonProperty("messages")]
    public IReadOnlyCollection<IReadOnlyCollection<Message>> NestedMessages { get; set; }

    [JsonIgnore] public IEnumerable<Message> Messages => NestedMessages.SelectMany(m => m);

    [JsonProperty("threads")]
    public Optional<IReadOnlyCollection<Channel>> Threads { get; set; }

    [JsonProperty("members")]
    public Optional<IReadOnlyCollection<ThreadMember>> ThreadMembers { get; set; }

    [JsonIgnore] public bool ParseMessages = true;
}
