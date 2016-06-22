using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Rest
{
    public class DeleteMessagesParams
    {
        [JsonProperty("messages")]
        public IEnumerable<ulong> MessageIds { get; set; }
        [JsonIgnore]
        public IEnumerable<IMessage> Messages { set { MessageIds = value.Select(x => x.Id); } }
    }
}
