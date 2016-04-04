namespace Discord
{
    public struct InviteGuild
    {
        /// <summary> Returns the unique identifier for this guild. </summary>
        public ulong Id { get; }
        /// <summary> Returns the name of this guild. </summary>
        public string Name { get; }

        internal InviteGuild(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
