using Discord.Gateway.Cache;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Discord.Gateway
{
    /// <summary>
    ///     Represents a general sticker received over the gateway.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketSticker : SocketCacheableEntity<ulong, IStickerModel>, ISticker
    {
        /// <inheritdoc/>
        public virtual ulong? PackId
            => _source.PackId;

        /// <inheritdoc/>
        public string Name
            => _source.Name;

        /// <inheritdoc/>
        public virtual string? Description
            => _source.Description;

        /// <inheritdoc/>
        public virtual IReadOnlyCollection<string> Tags
            => _tags.ToImmutableArray();

        /// <inheritdoc/>
        public virtual StickerType Type
            => _source.Type;

        /// <inheritdoc/>
        public StickerFormatType Format
            => _source.FormatType;

        /// <inheritdoc/>
        public virtual bool? IsAvailable
            => _source.Available;

        /// <inheritdoc/>
        public virtual int? SortOrder
            => _source.SortValue;

        /// <inheritdoc/>
        public string GetStickerUrl()
            => CDN.GetStickerUrl(Id, Format);

        protected override IStickerModel Model
            => _source;

        private IStickerModel _source;
        private string[] _tags;
        private int _tagsVersion;


        internal SocketSticker(DiscordGatewayClient client, IStickerModel model)
            : base(client, model.Id)
        {
            _source = model;
            UpdateTags(model);
        }

        [MemberNotNull(nameof(_tags))]
        private void UpdateTags(IStickerModel model)
        {
            if(_tags is null || _tagsVersion != model.Tags.GetHashCode())
            {
                _tags = model.Tags.Split(",");
                _tagsVersion = model.Tags.GetHashCode();
            }
        }

        internal override void Update(IStickerModel model)
            => _source = model;

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        internal string DebuggerDisplay => $"{Name} ({Id})";        
    }
}
