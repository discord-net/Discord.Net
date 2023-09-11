using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

public sealed class CreateStickerProperties
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public required Image File { get; set; }
}
