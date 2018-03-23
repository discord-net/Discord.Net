using System;

namespace Discord.Commands
{
    /// <summary> Sets priority of commands. </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PriorityAttribute : Attribute
    {
        /// <summary> Gets the priority which has been set for the command. </summary>
        public int Priority { get; }

        /// <summary> Creates a new <see cref="PriorityAttribute"/> with the given priority. </summary>
        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
