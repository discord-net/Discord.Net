using Model = Discord.API.Reaction;

namespace Discord
{
    public class RestReaction : IReaction
    {
        public Emoji Emoji { get; }
        public int Count { get; }
        public bool Me { get; }

        internal RestReaction(Emoji emoji, int count, bool me)
        {
            Emoji = emoji;
            Count = count;
            Me = me;
        }
        internal static RestReaction Create(Model model)
        {
            return new RestReaction(Emoji.Create(model.Emoji), model.Count, model.Me);
        }
    }
}
