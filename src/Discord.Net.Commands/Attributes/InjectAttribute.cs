using System;

namespace Discord.Commands
{
    /// <summary>
    /// Indicates that this property should be filled in by dependency injection.
    /// </summary>
    /// <remarks>
    /// This property **MUST** have a setter.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
    }
}
