using Model = Discord.API.EmbedProvider;

namespace Discord
{
    public struct EmbedProvider
    {
        public string Name { get; }
        public string Url { get; }

        public EmbedProvider(string name, string url)
        {
            Name = name;
            Url = url;
        }
        internal EmbedProvider(Model model)
            : this(model.Name, model.Url) { }
    }
}
