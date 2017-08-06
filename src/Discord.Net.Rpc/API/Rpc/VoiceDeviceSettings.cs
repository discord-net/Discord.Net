using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class VoiceDeviceSettings
    {
        [ModelProperty("device_id")]
        public Optional<string> DeviceId { get; set; }
        [ModelProperty("volume")]
        public Optional<float> Volume { get; set; }
        [ModelProperty("available_devices")]
        public Optional<VoiceDevice[]> AvailableDevices { get; set; }
    }
}
