using System.Collections.Generic;
using Model = Discord.API.Reaction;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST reaction object.
    /// </summary>
    public class RestReaction : IReaction
    {
        /// <inheritdoc />
        public IEmote Emote { get; }

        /// <summary>
        ///     Gets the number of reactions added.
        /// </summary>
        public int Count { get; }

        /// <summary>
        ///     Gets whether the reaction is added by the user.
        /// </summary>
        public bool Me { get; }

        /// <summary>
        ///     Gets whether the super-reaction is added by the user.
        /// </summary>
        public bool MeBurst { get; }

        /// <summary>
        ///     Gets the number of burst reactions added.
        /// </summary>
        public int BurstCount { get; }

        /// <summary>
        ///     Gets the number of normal reactions added.
        /// </summary>
        public int NormalCount { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<Color> BurstColors { get; }

        internal RestReaction(IEmote emote, int count, bool me, int burst, int normal, IReadOnlyCollection<Color> colors, bool meBurst)
        {
            Emote = emote;
            Count = count;
            Me = me;
            BurstCount = burst;
            NormalCount = normal;
            BurstColors = colors;
            MeBurst = meBurst;
        }
        internal static RestReaction Create(Model model)
        {
            IEmote emote;
            if (model.Emoji.Id.HasValue)
                emote = new Emote(model.Emoji.Id.Value, model.Emoji.Name, model.Emoji.Animated.GetValueOrDefault());
            else
                emote = new Emoji(model.Emoji.Name);
            return new RestReaction(emote,
                model.Count,
                model.Me,
                model.CountDetails.BurstCount,
                model.CountDetails.NormalCount,
                model.Colors.ToReadOnlyCollection(),
                model.MeBurst);
        }
    }
}
