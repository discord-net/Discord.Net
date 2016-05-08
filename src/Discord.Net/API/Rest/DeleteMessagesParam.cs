using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest
{
    public class DeleteMessagesParam
    {
        [JsonProperty("messages")]
        public IEnumerable<ulong> MessageIds { get; set; }
    }
}
