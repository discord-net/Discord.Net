using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Reaction;

namespace Discord
{
    public class RestReaction : IReaction
    {
        internal RestReaction(Model model)
        {
            _emoji = model.Emoji;
            _count = model.Count;

        }

        internal readonly API.Emoji _emoji;
        internal readonly int _count;
        internal readonly bool _me;

        public API.Emoji Emoji => _emoji;
        public int Count => _count;
        public bool Me => _me;
    }
}
