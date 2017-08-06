using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class VoiceDevice
    {
        [ModelProperty("id")]
        public string Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
    }
}
