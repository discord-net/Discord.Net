using System.Diagnostics;
using Model = Discord.API.Attachment;

namespace Discord
{
    /// <summary>
    ///     An attachment file seen in a <see cref="IUserMessage"/>.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Attachment : IAttachment
    {
        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public string Filename { get; }
        /// <inheritdoc />
        public string Url { get; }
        /// <inheritdoc />
        public string ProxyUrl { get; }
        /// <inheritdoc />
        public int Size { get; }
        /// <inheritdoc />
        public int? Height { get; }
        /// <inheritdoc />
        public int? Width { get; }

        internal Attachment(ulong id, string filename, string url, string proxyUrl, int size, int? height, int? width)
        {
            Id = id;
            Filename = filename;
            Url = url;
            ProxyUrl = proxyUrl;
            Size = size;
            Height = height;
            Width = width;
        }
        internal static Attachment Create(Model model)
        {
            return new Attachment(model.Id, model.Filename, model.Url, model.ProxyUrl, model.Size,
                model.Height.IsSpecified ? model.Height.Value : (int?)null,
                model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }

        /// <summary>
        ///     Returns the filename of this attachment.
        /// </summary>
        /// <returns>
        ///     A string containing the filename of this attachment.
        /// </returns>
        public override string ToString() => Filename;
        private string DebuggerDisplay => $"{Filename} ({Size} bytes)";
    }
}
