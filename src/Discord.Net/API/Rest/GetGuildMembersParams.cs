namespace Discord.API.Rest
{
    public class GetGuildMembersParams
    {
        internal Optional<int> _limit;
        public int Limit { set { _limit = value; } }

        internal Optional<ulong> _afterUserId;
        public ulong AfterUserId { set { _afterUserId = value; } }
    }
}
