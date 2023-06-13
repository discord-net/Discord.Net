using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class GuildFeatures
    {
        /// <summary>
        ///     Gets the flags of recognized features for this guild.
        /// </summary>
        public GuildFeature Value { get; }

        /// <summary>
        ///     Gets a collection of experimental features for this guild. Features that are not contained in <see cref="GuildFeature"/> are put in here.
        /// </summary>
        public IReadOnlyCollection<string> Experimental { get; }

        /// <summary>
        ///     Gets whether or not the guild has threads enabled.
        /// </summary>
        public bool HasThreads
            => HasFeature(GuildFeature.ThreadsEnabled | GuildFeature.ThreadsEnabledTesting);

        /// <summary>
        ///     Gets whether or not the guild has text-in-voice enabled.
        /// </summary>
        public bool HasTextInVoice
            => HasFeature(GuildFeature.TextInVoiceEnabled);

        /// <summary>
        ///     Gets whether or not the server is a internal staff server.
        /// </summary>
        /// <remarks>
        ///     You shouldn't touch anything here unless you know what you're doing :)
        /// </remarks>
        public bool IsStaffServer
            => HasFeature(GuildFeature.InternalEmployeeOnly);

        /// <summary>
        ///     Gets whether or not this server is a hub.
        /// </summary>
        public bool IsHub
            => HasFeature(GuildFeature.Hub);

        /// <summary>
        ///     Gets whether or this server is linked to a hub server.
        /// </summary>
        public bool IsLinkedToHub
            => HasFeature(GuildFeature.LinkedToHub);

        /// <summary>
        ///     Gets whether or not this server is partnered.
        /// </summary>
        public bool IsPartnered
            => HasFeature(GuildFeature.Partnered);

        /// <summary>
        ///     Gets whether or not this server is verified.
        /// </summary>
        public bool IsVerified
            => HasFeature(GuildFeature.Verified);

        /// <summary>
        ///     Gets whether or not this server has vanity urls enabled.
        /// </summary>
        public bool HasVanityUrl
            => HasFeature(GuildFeature.VanityUrl);

        /// <summary>
        ///     Gets whether or not this server has role subscriptions enabled.
        /// </summary>
        public bool HasRoleSubscriptions
            => HasFeature(GuildFeature.RoleSubscriptionsEnabled | GuildFeature.RoleSubscriptionsAvailableForPurchase);

        /// <summary>
        ///     Gets whether or not this server has role icons enabled.
        /// </summary>
        public bool HasRoleIcons
            => HasFeature(GuildFeature.RoleIcons);

        /// <summary>
        ///     Gets whether or not this server has private threads enabled.
        /// </summary>
        public bool HasPrivateThreads
            => HasFeature(GuildFeature.PrivateThreads);

        internal GuildFeatures(GuildFeature value, string[] experimental)
        {
            Value = value;
            Experimental = experimental.ToImmutableArray();
        }

        /// <summary>
        ///     Returns whether or not this guild has a feature.
        /// </summary>
        /// <param name="feature">The feature(s) to check for.</param>
        /// <returns><see langword="true"/> if this guild has the provided feature(s), otherwise <see langword="false"/>.</returns>
        public bool HasFeature(GuildFeature feature)
            => Value.HasFlag(feature);

        /// <summary>
        ///     Returns whether or not this guild has a feature.
        /// </summary>
        /// <param name="feature">The feature to check for.</param>
        /// <returns><see langword="true"/> if this guild has the provided feature, otherwise <see langword="false"/>.</returns>
        public bool HasFeature(string feature)
            => Experimental.Contains(feature);

        internal void EnsureFeature(GuildFeature feature)
        {
            if (!HasFeature(feature))
            {
                var vals = Enum.GetValues(typeof(GuildFeature)).Cast<GuildFeature>();

                var missingValues = vals.Where(x => feature.HasFlag(x) && !Value.HasFlag(x));

                throw new InvalidOperationException($"Missing required guild feature{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
            }
        }
    }
}
