using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.StickerItem;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a partial sticker received in a message.
    /// </summary>
    public class StickerItem : RestEntity<ulong>
    {
        /// <summary>
        ///     The name of this sticker.
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     The format of this sticker.
        /// </summary>
        public readonly StickerFormatType Format;

        internal StickerItem(BaseDiscordClient client, Model model)
            : base(client, model.Id)
        {
            this.Name = model.Name;
            this.Format = model.FormatType;
        }

        /// <summary>
        ///     Resolves this sticker item by fetching the <see cref="Sticker"/> from the API.
        /// </summary>
        /// <returns>
        ///     A task representing the download operation, the result of the task is a sticker object.
        /// </returns>

        public async Task<Sticker> ResolveStickerAsync()
        {
            var model = await Discord.ApiClient.GetStickerAsync(this.Id);

            return Sticker.Create(model);
        }
    }
}
