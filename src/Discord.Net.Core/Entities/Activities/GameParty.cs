namespace Discord
{
    public class GameParty
    {
        internal GameParty() { }

        public string Id { get; internal set; }
        public int Members { get; internal set; }
        public int Capacity { get; internal set; }
    }
}