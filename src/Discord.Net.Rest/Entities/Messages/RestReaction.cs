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
            Emoji = Emoji.FromApi(model.Emoji);
            Count = model.Count;
            Me = model.Me;
        }

        public Emoji Emoji { get; private set; }
        public int Count { get; private set; }
        public bool Me { get; private set; } 
    }
}
