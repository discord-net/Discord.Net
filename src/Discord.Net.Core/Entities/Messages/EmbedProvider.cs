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
        public static EmbedProvider Create(Model model)
        {
            return new EmbedProvider(model.Name, model.Url);
        }
    }
}
