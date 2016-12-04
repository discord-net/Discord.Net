namespace Discord
{
    public class ModifyGuildParams
    {
        public Optional<string> Username { get; set; }
        public Optional<string> Name { get; set; }
        public Optional<string> RegionId { get; set; }
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        public Optional<int> AfkTimeout { get; set; }
        public Optional<Image?> Icon { get; set; }
        public Optional<Image?> Splash { get; set; }
        public Optional<ulong?> AfkChannelId { get; set; }
        public Optional<ulong> OwnerId { get; set; }
    }
}
