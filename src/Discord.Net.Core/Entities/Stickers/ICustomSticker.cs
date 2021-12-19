using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a custom sticker within a guild.
    /// </summary>
    public interface ICustomSticker : ISticker
    {
        /// <summary>
        ///     Gets the users id who uploaded the sticker.
        /// </summary>
        /// <remarks>
        ///     In order to get the author id, the bot needs the MANAGE_EMOJIS_AND_STICKERS permission.
        /// </remarks>
        ulong? AuthorId { get; }

        /// <summary>
        ///     Gets the guild that this custom sticker is in.
        /// </summary>
        IGuild Guild { get; }

        /// <summary>
        ///     Modifies this sticker.
        /// </summary>
        /// <remarks>
        ///     This method modifies this sticker with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="StickerProperties"/>.
        ///     <br/>
        ///     <br/>
        ///     The bot needs the MANAGE_EMOJIS_AND_STICKERS permission within the guild in order to modify stickers.
        /// </remarks>
        /// <example>
        ///     <para>The following example replaces the name of the sticker with <c>kekw</c>.</para>
        ///     <code language="cs">
        ///     await sticker.ModifyAsync(x =&gt; x.Name = "kekw");
        ///     </code>
        /// </example>
        /// <param name="func">A delegate containing the properties to modify the sticker with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<StickerProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Deletes the current sticker.
        /// </summary>
        /// <remakrs>
        ///     The bot needs the MANAGE_EMOJIS_AND_STICKERS permission inside the guild in order to delete stickers.
        /// </remakrs>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///      A task that represents the asynchronous deletion operation.
        /// </returns>
        Task DeleteAsync(RequestOptions options = null);
    }
}
