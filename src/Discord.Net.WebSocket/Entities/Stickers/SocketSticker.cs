using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Sticker;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a general sticker received over the gateway.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSticker : SocketEntity<ulong>, ISticker
    {
        /// <inheritdoc/>
        public virtual ulong PackId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; protected set; }

        /// <inheritdoc/>
        public virtual string Description { get; private set; }

        /// <inheritdoc/>
        public virtual IReadOnlyCollection<string> Tags { get; private set; }

        /// <inheritdoc/>
        public virtual StickerType Type { get; private set; }

        /// <inheritdoc/>
        public StickerFormatType Format { get; protected set; }

        /// <inheritdoc/>
        public virtual bool? Available { get; protected set; }

        /// <inheritdoc/>
        public virtual int? SortOrder { get; private set; }

        /// <inheritdoc/>
        public string GetStickerUrl()
            => CDN.GetStickerUrl(Id, Format);

        internal SocketSticker(DiscordSocketClient client, ulong id)
            : base(client, id) { }

        internal static SocketSticker Create(DiscordSocketClient client, Model model)
        {
            SocketSticker entity;

            if (model.GuildId.IsSpecified)
                entity = new SocketCustomSticker(client, model.Id, client.GetGuild(model.GuildId.Value), model.User.IsSpecified ? model.User.Value.Id : null);
            else
                entity = new SocketSticker(client, model.Id);

            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            Name = model.Name;
            Description = model.Desription;
            PackId = model.PackId;
            Available = model.Available;
            Format = model.FormatType;
            Type = model.Type;
            SortOrder = model.SortValue;

            if (model.Tags.IsSpecified)
            {
                Tags = model.Tags.Value.Split(',').Select(x => x.Trim()).ToImmutableArray();
            }
            else
            {
                Tags = ImmutableArray<string>.Empty;
            }
        }

        internal string DebuggerDisplay => $"{Name} ({Id})";

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is API.Sticker stickerModel)
            {
                return stickerModel.Name == Name &&
                    stickerModel.Desription == Description &&
                    stickerModel.FormatType == Format &&
                    stickerModel.Id == Id &&
                    stickerModel.PackId == PackId &&
                    stickerModel.Type == Type &&
                    stickerModel.SortValue == SortOrder &&
                    stickerModel.Available == Available &&
                    (!stickerModel.Tags.IsSpecified || stickerModel.Tags.Value == string.Join(", ", Tags));
            }
            else
                return base.Equals(obj);
        }
    }
}
