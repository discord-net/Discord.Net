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
        /// <inheritdoc />
        public string Description { get; }
        /// <inheritdoc />
        public string ContentType { get; }
        /// <inheritdoc />
        public string Waveform { get; }
        /// <inheritdoc />
        public double? Duration { get; }

        /// <inheritdoc />
        public AttachmentFlags Flags { get; }

        internal Attachment(ulong id, string filename, string url, string proxyUrl, int size, int? height, int? width,
            bool? ephemeral, string description, string contentType, double? duration, string waveform, AttachmentFlags flags)
        {
            Id = id;
            Filename = filename;
            Url = url;
            ProxyUrl = proxyUrl;
            Size = size;
            Height = height;
            Width = width;
            Ephemeral = ephemeral.GetValueOrDefault(false);
            Description = description;
            ContentType = contentType;
            Duration = duration;
            Waveform = waveform;
            Flags = flags;
        }
        internal static Attachment Create(Model model)
        {
            return new Attachment(model.Id, model.Filename, model.Url, model.ProxyUrl, model.Size,
                model.Height.IsSpecified ? model.Height.Value : (int?)null,
                model.Width.IsSpecified ? model.Width.Value : (int?)null,
                model.Ephemeral.ToNullable(), model.Description.GetValueOrDefault(),
                model.ContentType.GetValueOrDefault(),
                model.DurationSeconds.IsSpecified ? model.DurationSeconds.Value : null,
                model.Waveform.GetValueOrDefault(null),
                model.Flags.GetValueOrDefault(AttachmentFlags.None));
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
