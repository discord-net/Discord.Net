#pragma warning disable CS1591
namespace Discord.API.Rest
{
    public class GetGuildMembersParams
    {
        internal Optional<int> _limit { get; set; }
        public int Limit { set { _limit = value; } }

        internal Optional<ulong> _afterUserId { get; set; }
        public ulong AfterUserId { set { _afterUserId = value; } }
    }
}
