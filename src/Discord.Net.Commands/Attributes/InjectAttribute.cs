using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
    }
}
