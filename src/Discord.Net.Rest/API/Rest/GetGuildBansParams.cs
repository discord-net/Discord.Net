namespace Discord.API.Rest
{
    internal class GetGuildBansParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<Direction> RelativeDirection { get; set; }
        public Optional<ulong> RelativeUserId { get; set; }
    }
}
