using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    public class MessageActivity : IMessageActivityModel
    {
        [JsonProperty("type")]
        public Optional<MessageActivityType> Type { get; set; }
        [JsonProperty("party_id")]
        public Optional<string> PartyId { get; set; }

        MessageActivityType? IMessageActivityModel.Type { get => Type.ToNullable(); set => throw new NotSupportedException(); }
        string IMessageActivityModel.PartyId { get => PartyId.GetValueOrDefault(); set => throw new NotSupportedException(); }
    }
}
