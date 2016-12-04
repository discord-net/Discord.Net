namespace Discord
{
    public class ModifyGuildChannelsParams
    {
        public ulong Id { get; set; }
        public int Position { get; set; }

        public ModifyGuildChannelsParams(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
