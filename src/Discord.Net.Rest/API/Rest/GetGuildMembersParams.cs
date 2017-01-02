#pragma warning disable CS1591
namespace Discord.API.Rest
{
    internal class GetGuildMembersParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> AfterUserId { get; set; }
    }
}
