using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    public interface IReaction
    {
        API.Emoji Emoji { get; }
    }
}
