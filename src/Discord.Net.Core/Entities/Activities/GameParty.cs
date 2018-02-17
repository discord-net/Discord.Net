namespace Discord
{
    public class GameParty
    {
        internal GameParty() { }

        public string Id { get; internal set; }
        public long Members { get; internal set; }
        public long Capacity { get; internal set; }
    }
}
