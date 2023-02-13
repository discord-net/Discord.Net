namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a Slash Command parameter choice.
    /// </summary>
    public class ParameterChoice
    {
        /// <summary>
        ///     Gets the name of the choice.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the value of the choice.
        /// </summary>
        public object Value { get; }

        internal ParameterChoice(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
