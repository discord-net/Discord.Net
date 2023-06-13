using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Overrides
{
    /// <summary>
    ///     Represents context that's passed to an override in the initialization step.
    /// </summary>
    public sealed class OverrideContext
    {
        /// <summary>
        ///     A callback used to log messages.
        /// </summary>
        public Action<string> Log { get; private set; }

        /// <summary>
        ///     The info about the override.
        /// </summary>
        public Override Info { get; private set; }

        internal OverrideContext(Action<string> log, Override info)
        {
            Log = log;
            Info = info;
        }
    }
}
