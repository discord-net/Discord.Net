namespace Discord
{
    internal class Emoji : IEmote
    {
        public Emoji(string name)
        {
            // TODO: validation?
            Name = name;
        }

        public string Name { get; set; }
        public string Mention => Name;
    }
}
