using Model = Discord.API.Reaction;

namespace Discord.Rest
{
    public class RestReaction : IReaction
    {
        internal RestReaction(IEmote emote, int count, bool me)
        {
            Emote = emote;
            Count = count;
            Me = me;
        }

        public int Count { get; }
        public bool Me { get; }
        public IEmote Emote { get; }

        internal static RestReaction Create(Model model)
        {
            IEmote emote;
            if (model.Emoji.Id.HasValue)
                emote = new Emote(model.Emoji.Id.Value, model.Emoji.Name, model.Emoji.Animated.GetValueOrDefault());
            else
                emote = new Emoji(model.Emoji.Name);
            return new RestReaction(emote, model.Count, model.Me);
        }
    }
}
