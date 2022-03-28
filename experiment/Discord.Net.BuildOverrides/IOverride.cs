using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Overrides
{
    /// <summary>
    ///     Represents a generic build override for Discord.Net
    /// </summary>
    public interface IOverride
    {
        /// <summary>
        ///     Initializes the override.
        /// </summary>
        /// <remarks>
        ///     This method is called by the <see cref="BuildOverrides"/> class
        ///     and should not be called externally from it.
        /// </remarks>
        /// <param name="context">Context used by an override to initialize.</param>
        /// <returns>
        ///     A task representing the asynchronous initialization operation.
        /// </returns>
        Task InitializeAsync(OverrideContext context);

        /// <summary>
        ///     Registers a callback to load a dependency for this override.
        /// </summary>
        /// <param name="func">The callback to load an external dependency.</param>
        void RegisterPackageLookupHandler(Func<string, Task<Assembly>> func);
    }
}
