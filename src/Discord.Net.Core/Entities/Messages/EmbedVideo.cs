using System.Diagnostics;
using Model = Discord.API.EmbedVideo;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedVideo
    {
        public string Url { get; }
        public int? Height { get; }
        public int? Width { get; }

        private EmbedVideo(string url, int? height, int? width)
        {
            Url = url;
            Height = height;
            Width = width;
        }
        internal static EmbedVideo Create(Model model)
        {
            return new EmbedVideo(model.Url,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }

        private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";
        public override string ToString() => Url;
    }
}
