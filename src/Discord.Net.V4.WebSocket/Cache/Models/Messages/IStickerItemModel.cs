using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IStickerItemModel
    {
        ulong Id { get; }
        string Name { get; }
        StickerFormatType FormatType { get; }
    }
}
