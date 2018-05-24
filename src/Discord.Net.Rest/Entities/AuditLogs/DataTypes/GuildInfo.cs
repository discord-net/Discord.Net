namespace Discord.Rest
{
    public struct GuildInfo
    {
        internal GuildInfo(int? afkTimeout, DefaultMessageNotifications? defaultNotifs,
            ulong? afkChannel, string name, string region, string icon,
            VerificationLevel? verification, IUser owner, MfaLevel? mfa, int? filter)
        {
            AfkTimeout = afkTimeout;
            DefaultMessageNotifications = defaultNotifs;
            AfkChannelId = afkChannel;
            Name = name;
            RegionId = region;
            IconHash = icon;
            VerificationLevel = verification;
            Owner = owner;
            MfaLevel = mfa;
            ContentFilterLevel = filter;
        }

        public int? AfkTimeout { get; }
        public DefaultMessageNotifications? DefaultMessageNotifications { get; }
        public ulong? AfkChannelId { get; }
        public string Name { get; }
        public string RegionId { get; }
        public string IconHash { get; }
        public VerificationLevel? VerificationLevel { get; }
        public IUser Owner { get; }
        public MfaLevel? MfaLevel { get; }
        public int? ContentFilterLevel { get; }
    }
}
