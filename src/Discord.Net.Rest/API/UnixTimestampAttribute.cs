using System;

namespace Discord.API
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class UnixTimestampAttribute : Attribute { }
}