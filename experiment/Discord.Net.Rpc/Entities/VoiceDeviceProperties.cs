namespace Discord.Rpc
{
    public class VoiceDeviceProperties
    {
        public Optional<string> DeviceId { get; set; }
        public Optional<float> Volume { get; set; }
        public Optional<VoiceDevice[]> AvailableDevices { get; set; }
    }
}
