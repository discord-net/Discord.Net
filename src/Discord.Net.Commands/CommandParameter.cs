namespace Discord.Commands
{
    public enum ParameterType
    {
        /// <summary> Catches a single required parameter. </summary>
        Required,
        /// <summary> Catches a single optional parameter. </summary>
        Optional,
        /// <summary> Catches a zero or more optional parameters. </summary>
        Multiple,
        /// <summary> Catches all remaining text as a single optional parameter. </summary>
        Unparsed
    }
    public class CommandParameter
    {
        public string Name { get; }
        public int Id { get; internal set; }
        public ParameterType Type { get; }

        internal CommandParameter(string name, ParameterType type)
        {
            Name = name;
            Type = type;
        }
    }
}
