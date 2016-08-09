#pragma warning disable CS1591
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DeleteMessagesParams
    {
        [JsonProperty("messages")]
        internal ulong[] _messages { get; set; }
        public IEnumerable<ulong> MessageIds { set { _messages = value.ToArray(); } }
        public IEnumerable<IMessage> Messages { set { _messages = value.Select(x => x.Id).ToArray(); } }
    }
}
