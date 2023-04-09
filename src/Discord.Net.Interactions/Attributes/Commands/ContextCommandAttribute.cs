using System;
using System.Reflection;

namespace Discord.Interactions
{
    /// <summary>
    ///     Base attribute for creating a Context Commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ContextCommandAttribute : Attribute
    {
        /// <summary>
        ///     Gets the name of this Context Command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the type of this Context Command.
        /// </summary>
        public ApplicationCommandType CommandType { get; }

        /// <summary>
        ///     Gets the run mode this command gets executed with.
        /// </summary>
        public RunMode RunMode { get; }

        internal ContextCommandAttribute(string name, ApplicationCommandType commandType, RunMode runMode = RunMode.Default)
        {
            Name = name;
            CommandType = commandType;
            RunMode = runMode;
        }

        internal virtual void CheckMethodDefinition(MethodInfo methodInfo) { }
    }
}
