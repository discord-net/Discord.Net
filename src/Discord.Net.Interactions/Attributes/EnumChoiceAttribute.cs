using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Customize the displayed value of a slash command choice enum. Only works with the default enum type converter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ChoiceDisplayAttribute : Attribute
    {
        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        public string Name { get; } = null;

        /// <summary>
        ///     Modify the default name and description values of a Slash Command parameter.
        /// </summary>
        /// <param name="name">Name of the parameter.</param>
        public ChoiceDisplayAttribute(string name)
        {
            Name = name;
        }
    }
}
