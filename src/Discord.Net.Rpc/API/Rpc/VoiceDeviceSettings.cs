using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class VoiceDeviceSettings
    {
        [JsonProperty("device_id")]
        public string DeviceId { get; set; }
        [JsonProperty("volume")]
        public float Volume { get; set; }
        [JsonProperty("available_devices")]
        public VoiceDevice[] AvailableDevices { get; set; }
    }
}
