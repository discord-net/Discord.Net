using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageComponentModel
    {
        ComponentType Type { get; set; }
        string CustomId { get; set; }
        bool? Disabled { get; set; }
        ButtonStyle? Style { get; set; }
        string Label { get; set; }

        // emoji
        ulong? EmojiId { get; set; }
        string EmojiName { get; set; }
        bool? EmojiAnimated { get; set; }

        string Url { get; set; }

        IMessageComponentOptionModel[] Options { get; set; }

        string Placeholder { get; set; }
        int? MinValues { get; set; }
        int? MaxValues { get; set; }
        IMessageComponentModel[] Components { get; set; }
        int? MinLength { get; set; }
        int? MaxLength { get; set; }
        bool? Required { get; set; }
        string Value { get; set; }
    }
}
