using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create nested Slash Commands by marking a module as a command group.
    /// </summary>
    /// <remarks>
    ///     <see cref="ContextCommandAttribute"/> commands wil not be affected by this.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        /// <summary>
        ///     Gets the name of the group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the description of the group.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Create a command group.
        /// </summary>
        /// <param name="name">Name of the group.</param>
        /// <param name="description">Description of the group.</param>
        public GroupAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
