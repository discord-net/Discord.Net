using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IStickerItemModel
    {
        ulong Id { get; set; }
        string Name { get; set; }
        StickerFormatType Format { get; set; }
    }
}
