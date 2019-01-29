namespace Discord
{
    // placeholder for user-constructed guild emotes
    // TODO: naming - should be called GuildEmote? but does not impl IGuildEmote, so maybe not...
    internal class Emote : IEmote
    {
        public Emote(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        public ulong Id { get; set; }
        public string Name { get; set; }

        public string Mention => EmoteUtilities.FormatGuildEmote(Id, Name);
    }
}
