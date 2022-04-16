using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal static class StateExtensions
    {
        public static StateBehavior ToBehavior(this CacheMode mode)
        {
            return mode switch
            {
                CacheMode.AllowDownload => StateBehavior.AllowDownload,
                CacheMode.CacheOnly => StateBehavior.CacheOnly,
                _ => StateBehavior.AllowDownload
            };
        }
    }
}
