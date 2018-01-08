using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class RemainderAttribute : Attribute
    {
    }
}
