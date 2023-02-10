using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create an Slash Application Command.
    /// </summary>
    /// <remarks>
    ///     <see cref="GroupAttribute"/> prefix will be used to created nested Slash Application Commands.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlashCommandAttribute : Attribute
    {
        /// <summary>
        ///     Gets the name of the Slash Command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the description of the Slash Command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command.
        /// </summary>
        public bool IgnoreGroupNames { get; }

        /// <summary>
        ///     Gets the run mode this command gets executed with.
        /// </summary>
        public RunMode RunMode { get; }

        /// <summary>
        ///     Register a method as a Slash Command.
        /// </summary>
        /// <param name="name">Name of the command.</param>
        /// <param name="description">Description of the command.</param>
        /// <param name="ignoreGroupNames"> If <see langword="true"/>, <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command.</param>
        /// <param name="runMode">Set the run mode of the command.</param>
        public SlashCommandAttribute(string name, string description, bool ignoreGroupNames = false, RunMode runMode = RunMode.Default)
        {
            Name = name;
            Description = description;
            IgnoreGroupNames = ignoreGroupNames;
            RunMode = runMode;
        }
    }
}
