using Discord.Rest;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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
        public byte[] WaveformBytes { get; }
        /// <inheritdoc />
        public double? Duration { get; }

        /// <inheritdoc cref="IAttachment.ClipParticipants" />
        public IReadOnlyCollection<RestUser> ClipParticipants { get; }

        /// <inheritdoc />
        public string Title { get; }

        /// <inheritdoc />
        public DateTimeOffset? ClipCreatedAt { get; }

        /// <inheritdoc />
        public AttachmentFlags Flags { get; }

        internal Attachment(ulong id, string filename, string url, string proxyUrl, int size, int? height, int? width,
            bool? ephemeral, string description, string contentType, double? duration, string waveform, AttachmentFlags flags, string title,
            IReadOnlyCollection<RestUser> clipParticipants, DateTimeOffset? clipCreatedAt)
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
            Title = title;
            ClipParticipants = clipParticipants;
            ClipCreatedAt = clipCreatedAt;
            if (waveform is not null)
                WaveformBytes = Convert.FromBase64String(waveform);
        }

        internal static Attachment Create(Model model, BaseDiscordClient discord)
        {
            return new Attachment(model.Id, model.Filename, model.Url, model.ProxyUrl, model.Size,
                model.Height.IsSpecified ? model.Height.Value : null,
                model.Width.IsSpecified ? model.Width.Value : null,
                model.Ephemeral.ToNullable(), model.Description.GetValueOrDefault(),
                model.ContentType.GetValueOrDefault(),
                model.DurationSeconds.IsSpecified ? model.DurationSeconds.Value : null,
                model.Waveform.GetValueOrDefault(null),
                model.Flags.GetValueOrDefault(AttachmentFlags.None),
                model.Title.GetValueOrDefault(null),
                model.ClipParticipants.GetValueOrDefault(Array.Empty<API.User>()).Select(x => RestUser.Create(discord, x)).ToImmutableArray(),
                model.ClipCreatedAt.IsSpecified ? model.ClipCreatedAt.Value : null);
        }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        /// <summary>
        ///     Returns the filename of this attachment.
        /// </summary>
        /// <returns>
        ///     A string containing the filename of this attachment.
        /// </returns>
        public override string ToString() => Filename;
        private string DebuggerDisplay => $"{Filename} ({Size} bytes)";

        /// <inheritdoc />
        IReadOnlyCollection<IUser> IAttachment.ClipParticipants => ClipParticipants;
    }
}
