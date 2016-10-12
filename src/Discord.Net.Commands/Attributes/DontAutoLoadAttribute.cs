using System;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DontAutoLoadAttribute : Attribute
    {
    }
}
