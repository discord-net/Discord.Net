using System;

namespace Discord.Commands
{
    /// <summary>
    ///     Prevents the marked property from being injected into a module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DontInjectAttribute : Attribute
    {
    }
}
