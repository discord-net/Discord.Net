using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public enum CacheRunMode
    {
        /// <summary>
        ///     The cache should preform a synchronous cache lookup.
        /// </summary>
        Sync,

        /// <summary>
        ///     The cache should preform either a <see cref="Sync"/> or asynchronous cache lookup.
        /// </summary>
        Async
    }
}
