using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest
{
    public class DeleteMessagesParams
    {
        [JsonProperty("messages")]
        public IEnumerable<ulong> MessageIds { get; set; }
    }
}
