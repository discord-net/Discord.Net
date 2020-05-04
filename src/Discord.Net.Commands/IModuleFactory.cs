using System;
using System.Reflection;

namespace Discord.Commands
{
    /// <summary>
    ///     Provides a custom logic for creating module instances.
    /// </summary>
    public interface IModuleFactory
    {
        /// <summary>
        ///     Creates a builder function for the provided <see cref="TypeInfo" />.
        /// </summary>
        /// <param name="typeInfo">The module's type information.</param>
        /// <param name="commands">The <see cref="CommandService" /> that requested a new module instance.</param>
        /// <returns>
        ///     A factory function for the provided module type. 
        /// </returns>
        Func<object> CreateBuilder(TypeInfo typeInfo, CommandService commands);
    }
}
