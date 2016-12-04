namespace Discord
{
    public class ModifyVoiceChannelParams : ModifyGuildChannelParams
    {
        public Optional<int> Bitrate { get; set; }
        public Optional<int> UserLimit { get; set; }
    }
}
