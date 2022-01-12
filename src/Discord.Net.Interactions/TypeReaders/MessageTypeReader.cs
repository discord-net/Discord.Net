using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     A <see cref="TypeReader"/> for parsing objects implementing <see cref="IMessage"/>.
    /// </summary>
    /// <typeparam name="T">The type to be checked; must implement <see cref="IMessage"/>.</typeparam>
    internal class MessageTypeReader<T> : TypeReader<T> where T : class, IMessage
    {
        /// <inheritdoc />
        public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, object input, IServiceProvider services)
        {
            if (ulong.TryParse(input as string, NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
            {
                if (await context.Channel.GetMessageAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) is T msg)
                    return TypeConverterResult.FromSuccess(msg);
            }

            return TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Message not found.");
        }
    }
}
