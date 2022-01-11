using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IRestInteractionContext : IInteractionContext
    {
        /// <summary>
        ///     Gets or sets the callback to use when the service has outgoing json for the rest webhook.
        /// </summary>
        /// <remarks>
        ///     If this property is <see langword="null"/> the default callback will be used.
        /// </remarks>
        Func<string, Task> InteractionResponseCallback { get; }
    }
}
