using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IReactionMetadataModel
    {
        IEmojiModel Emoji { get; set; }
        ulong[] Users { get; set; }
    }
}
