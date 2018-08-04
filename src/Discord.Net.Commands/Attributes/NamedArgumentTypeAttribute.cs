using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NamedArgumentTypeAttribute : Attribute { }
}
