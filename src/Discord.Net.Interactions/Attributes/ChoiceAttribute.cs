using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Add a pre-determined argument value to a command parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class ChoiceAttribute : Attribute
    {
        /// <summary>
        ///     Gets the name of the choice.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the type of this choice.
        /// </summary>
        public SlashCommandChoiceType Type { get; }

        /// <summary>
        ///     Gets the value that will be used whenever this choice is selected.
        /// </summary>
        public object Value { get; }

        private ChoiceAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Create a parameter choice with type <see cref="SlashCommandChoiceType.String"/>.
        /// </summary>
        /// <param name="name">Name of the choice.</param>
        /// <param name="value">Predefined value of the choice.</param>
        public ChoiceAttribute(string name, string value) : this(name)
        {
            Type = SlashCommandChoiceType.String;
            Value = value;
        }

        /// <summary>
        ///     Create a parameter choice with type <see cref="SlashCommandChoiceType.Integer"/>.
        /// </summary>
        /// <param name="name">Name of the choice.</param>
        /// <param name="value">Predefined value of the choice.</param>
        public ChoiceAttribute(string name, int value) : this(name)
        {
            Type = SlashCommandChoiceType.Integer;
            Value = value;
        }

        /// <summary>
        ///     Create a parameter choice with type <see cref="SlashCommandChoiceType.Number"/>.
        /// </summary>
        /// <param name="name">Name of the choice.</param>
        /// <param name="value">Predefined value of the choice.</param>
        public ChoiceAttribute(string name, double value) : this(name)
        {
            Type = SlashCommandChoiceType.Number;
            Value = value;
        }
    }
}
