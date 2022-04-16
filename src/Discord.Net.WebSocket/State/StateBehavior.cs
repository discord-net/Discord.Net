using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public enum StateBehavior
    {
        /// <summary>
        ///     Use the default Cache Behavior of the client.
        /// </summary>
        /// <seealso cref="DiscordSocketConfig.DefaultStateBehavior"/>
        Default = 0,
        /// <summary>
        ///     The entity will only be retrieved via a synchronous cache lookup.
        ///
        ///     For the default <see cref="IStateProvider"/>, this is equivalent to using <see cref="CacheOnly"/>
        /// </summary>
        /// <remarks>
        ///     This flag is used to indicate that the retrieval of this entity should not leave the
        ///     synchronous path of the <see cref="System.Threading.Tasks.ValueTask"/>. When true,
        ///     the calling method *should* not ever leave the calling task, and never generate an async
        ///     state machine.
        ///
        ///     Bear in mind that the true behavior of this flag depends entirely on the <see cref="IStateProvider"/> to
        ///     abide by design implications of this flag. Once Discord.Net has called out to the state provider with this
        ///     flag, it is out of our control whether or not an async method is evaluated.
        /// </remarks>
        SyncOnly = 1,
        /// <summary>
        ///     The entity will only be retrieved via a cache lookup - the Discord API will not be contacted to retrieve the entity.
        /// </summary>
        /// <remarks>
        ///     When using an alternative <see cref="IStateProvider"/>, usage of this flag implies that it is
        ///     okay for the state provider to make an external call if the local cache missed the entity.
        ///
        ///     Note that when designing an <see cref="IStateProvider"/>, this flag does not imply that the state
        ///     provider itself should contact Discord for the entity; rather that if using a dual-layer caching system,
        ///     it would be okay to contact an external layer, e.g. Redis, for the entity.
        /// </remarks>
        CacheOnly = 2,
        /// <summary>
        ///     The entity will be downloaded from the Discord REST API if the <see cref="ICacheProvider"/> on hand cannot locate it.
        /// </summary>
        AllowDownload = 3,
        /// <summary>
        ///     The entity will be downloaded from the Discord REST API. The local <see cref="ICacheProvider"/> will not be contacted to find the entity.
        /// </summary>
        DownloadOnly = 4
    }
}
