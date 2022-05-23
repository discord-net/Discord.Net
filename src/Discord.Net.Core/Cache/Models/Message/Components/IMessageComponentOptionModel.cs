using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageComponentOptionModel
    {
        string Label { get; set; }
        string Value { get; set; }
        string Description { get; set; }

        // emoji
        ulong? EmojiId { get; set; }
        string EmojiName { get; set; }
        bool? EmojiAnimated { get; set; }

        bool? Default { get; set; }
    }
}
