using System;

namespace Discord.Commands
{
    /// <summary>
    ///     Marks the input to not be parsed by the parser.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RemainderAttribute : Attribute
    {
    }
}
