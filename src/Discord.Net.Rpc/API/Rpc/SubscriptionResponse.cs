#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class SubscriptionResponse
    {
        [ModelProperty("evt")]
        public string Event { get; set; }
    }
}
