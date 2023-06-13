using System;
using System.Runtime.CompilerServices;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create a Message Component interaction handler, CustomId represents
    ///     the CustomId of the Message Component that will be handled.
    /// </summary>
    /// <remarks>
    ///     <see cref="GroupAttribute"/>s will add prefixes to this command if <see cref="IgnoreGroupNames"/> is set to <see langword="false"/>
    ///     CustomID supports a Wild Card pattern where you can use the <see cref="InteractionServiceConfig.WildCardExpression"/> to match a set of CustomIDs.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ComponentInteractionAttribute : Attribute
    {
        /// <summary>
        ///     Gets the string to compare the Message Component CustomIDs with.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets <see langword="true"/> if <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command.
        /// </summary>
        public bool IgnoreGroupNames { get; }

        /// <summary>
        ///     Gets the run mode this command gets executed with.
        /// </summary>
        public RunMode RunMode { get; }

        /// <summary>
        ///     Gets or sets whether the <see cref="CustomId"/> should be treated as a raw Regex pattern.
        /// </summary>
        /// <remarks>
        ///     <see langword="false"/> defaults to the pattern used before 3.9.0.
        /// </remarks>
        public bool TreatAsRegex { get; set; } = false;

        /// <summary>
        ///     Create a command for component interaction handling.
        /// </summary>
        /// <param name="customId">String to compare the Message Component CustomIDs with.</param>
        /// <param name="ignoreGroupNames">If <see langword="true"/> <see cref="GroupAttribute"/>s will be ignored while creating this command and this method will be treated as a top level command.</param>
        /// <param name="runMode">Set the run mode of the command.</param>
        public ComponentInteractionAttribute(string customId, bool ignoreGroupNames = false, RunMode runMode = RunMode.Default)
        {
            CustomId = customId;
            IgnoreGroupNames = ignoreGroupNames;
            RunMode = runMode;
        }
    }
}
