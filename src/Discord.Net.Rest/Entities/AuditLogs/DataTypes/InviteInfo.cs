namespace Discord.Rest
{
    public struct InviteInfo
    {
        internal InviteInfo(int? maxAge, string code, bool? temporary, ulong? channelId, int? maxUses)
        {
            MaxAge = maxAge;
            Code = code;
            Temporary = temporary;
            ChannelId = channelId;
            MaxUses = maxUses;
        }

        public int? MaxAge { get; }
        public string Code { get; }
        public bool? Temporary { get; }
        public ulong? ChannelId { get; }
        public int? MaxUses { get; }
    }
}
