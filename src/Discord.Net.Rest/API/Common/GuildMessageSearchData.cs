using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API;

internal class GuildMessageSearchData
{
    // Regular payload
    [JsonProperty("doing_deep_historical_index")]
    public Optional<bool> DoingDeepHistoricalIndex { get; set; }

    [JsonProperty("documents_indexed")]
    public Optional<int> DocumentsIndexed { get; set; }

    [JsonProperty("total_results")]
    public Optional<int> TotalResults { get; set; }

    [JsonProperty("messages")]
    public Optional<Message[][]> NestedMessages { get; set; }

    [JsonIgnore] public IEnumerable<Message> Messages => NestedMessages.GetValueOrDefault([]).SelectMany(m => m);

    [JsonProperty("threads")]
    public Optional<IReadOnlyCollection<Channel>> Threads { get; set; }

    [JsonProperty("members")]
    public Optional<IReadOnlyCollection<ThreadMember>> ThreadMembers { get; set; }

    // Error response
    [JsonProperty("message")]
    public Optional<string> ErrorMessage { get; set; }

    [JsonProperty("code")]
    public Optional<string> Code { get; set; }
    
    [JsonProperty("retry_after")]
    public Optional<int> RetryAfter { get; set; }
}
