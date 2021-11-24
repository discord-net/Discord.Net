using System.Diagnostics;
using Model = Discord.API.Attachment;

namespace Discord
{
    /// <inheritdoc cref="IAttachment"/>
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
        /// <inheritdoc />
        public bool Ephemeral { get; }

        internal Attachment(ulong id, string filename, string url, string proxyUrl, int size, int? height, int? width, bool? ephemeral)
        {
            Id = id;
            Filename = filename;
            Url = url;
            ProxyUrl = proxyUrl;
            Size = size;
            Height = height;
            Width = width;
            Ephemeral = ephemeral.GetValueOrDefault(false);
        }
        internal static Attachment Create(Model model)
        {
            return new Attachment(model.Id, model.Filename, model.Url, model.ProxyUrl, model.Size,
                model.Height.IsSpecified ? model.Height.Value : (int?)null,
                model.Width.IsSpecified ? model.Width.Value : (int?)null,
                model.Ephemeral.ToNullable());
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
