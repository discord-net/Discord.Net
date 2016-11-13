using System.Diagnostics;
using Model = Discord.API.EmbedField;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Inline { get; set; }

        private EmbedField(string name, string value, bool inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
        internal static EmbedField Create(Model model)
        {
            return new EmbedField(model.Name, model.Value, model.Inline);
        }

        private string DebuggerDisplay => $"{Name} ({Value}";
        public override string ToString() => Name;
    }
}
