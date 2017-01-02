using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class VoiceDeviceSettings
    {
        [JsonProperty("device_id")]
        public Optional<string> DeviceId { get; set; }
        [JsonProperty("volume")]
        public Optional<float> Volume { get; set; }
        [JsonProperty("available_devices")]
        public Optional<VoiceDevice[]> AvailableDevices { get; set; }
    }
}
