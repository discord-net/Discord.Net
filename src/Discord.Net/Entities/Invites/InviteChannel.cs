namespace Discord
{
    public struct InviteChannel
    {
        /// <summary> Returns the unique identifier for this channel. </summary>
        public ulong Id { get; }
        /// <summary> Returns the name of this channel. </summary>
        public string Name { get; }

        internal InviteChannel(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
