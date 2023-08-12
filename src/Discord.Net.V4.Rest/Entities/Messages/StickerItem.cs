using System.Threading.Tasks;
using Model = Discord.API.StickerItem;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a partial sticker received in a message.
    /// </summary>
    public class StickerItem : RestEntity<ulong>, IStickerItem
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public StickerFormatType Format { get; }

        internal StickerItem(BaseDiscordClient client, Model model)
            : base(client, model.Id)
        {
            Name = model.Name;
            Format = model.FormatType;
        }

        /// <summary>
        ///     Resolves this sticker item by fetching the <see cref="Sticker"/> from the API.
        /// </summary>
        /// <returns>
        ///     A task representing the download operation, the result of the task is a sticker object.
        /// </returns>
        public async Task<Sticker> ResolveStickerAsync()
        {
            var model = await Discord.ApiClient.GetStickerAsync(Id);

            return model.GuildId.IsSpecified
                ? CustomSticker.Create(Discord, model, model.GuildId.Value, model.User.IsSpecified ? model.User.Value.Id : null)
                : Sticker.Create(Discord, model);
        }
    }
}
