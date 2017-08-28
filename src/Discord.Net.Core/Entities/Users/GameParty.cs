namespace Discord
{
    public struct GameParty
    {
        public ulong[] Size { get; }
        public ulong Id { get; }

        public GameParty(ulong[] size, ulong id)
        {
            Size = size;
            Id = id;
        }
    }
}