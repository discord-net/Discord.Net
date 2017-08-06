using Discord.Rpc;
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class VoiceShortcut
    {
        [ModelProperty("type")]
        public Optional<VoiceShortcutType> Type { get; set; }
        [ModelProperty("code")]
        public Optional<int> Code { get; set; }
        [ModelProperty("name")]
        public Optional<string> Name { get; set; }
    }
}
