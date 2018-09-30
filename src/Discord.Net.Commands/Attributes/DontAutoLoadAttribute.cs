using System;

namespace Discord.Commands
{
    /// <summary>
    ///     Prevents the marked module from being loaded automatically.
    /// </summary>
    /// <remarks>
    ///     This attribute tells <see cref="CommandService" /> to ignore the marked module from being loaded
    ///     automatically (e.g. the <see cref="CommandService.AddModulesAsync" /> method). If a non-public module marked
    ///     with this attribute is attempted to be loaded manually, the loading process will also fail.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DontAutoLoadAttribute : Attribute
    {
    }
}
