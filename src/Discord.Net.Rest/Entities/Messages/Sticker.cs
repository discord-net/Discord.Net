using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Model = Discord.API.Sticker;

namespace Discord.Rest
{
    /// <inheritdoc cref="ISticker"/>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Sticker : RestEntity<ulong>, ISticker
    {
        /// <inheritdoc />
        public ulong PackId { get; protected set; }
        /// <inheritdoc />
        public string Name { get; protected set; }
        /// <inheritdoc />
        public string Description { get; protected set; }
        /// <inheritdoc />
        public IReadOnlyCollection<string> Tags { get; protected set; }
        /// <inheritdoc />
        public StickerType Type { get; protected set; }
        /// <inheritdoc />
        public bool? IsAvailable { get; protected set; }
        /// <inheritdoc />
        public int? SortOrder { get; protected set; }
        /// <inheritdoc />
        public StickerFormatType Format { get; protected set; }

        /// <inheritdoc/>
        public string GetStickerUrl()
            => CDN.GetStickerUrl(Id, Format);

        internal Sticker(BaseDiscordClient client, ulong id)
            : base(client, id) { }
        internal static Sticker Create(BaseDiscordClient client, Model model)
        {
            var entity = new Sticker(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            PackId = model.PackId;
            Name = model.Name;
            Description = model.Description;
            Tags = model.Tags.IsSpecified ? model.Tags.Value.Split(',').Select(x => x.Trim()).ToArray() : Array.Empty<string>();
            Type = model.Type;
            SortOrder = model.SortValue;
            IsAvailable = model.Available;
            Format = model.FormatType;
        }

        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
