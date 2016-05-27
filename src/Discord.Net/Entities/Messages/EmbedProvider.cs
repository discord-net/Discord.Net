using Model = Discord.API.EmbedProvider;

namespace Discord
{
    public struct EmbedProvider
    {
        public string Name { get; }
        public string Url { get; }

        internal EmbedProvider(Model model)
        {
            Name = model.Name;
            Url = model.Url;
        }
    }
}
