using System;

namespace Discord.Commands
{
    /// <summary> Marks the module as a command group. </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        /// <summary> Gets the prefix set for the module. </summary>
        public string Prefix { get; }

        public GroupAttribute()
        {
            Prefix = null;
        }
        /// <summary> Creates a <see cref="GroupAttribute"/> with the provided prefix. </summary>
        /// <param name="prefix"> The prefix of the module group. </param>
        public GroupAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
