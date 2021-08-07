using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System;
using Model = Discord.API.Team;

namespace Discord.Rest
{
    public class RestTeam : RestEntity<ulong>, ITeam
    {
        /// <inheritdoc />
        [Obsolete("This property is obsolete. Call GetIconUrl instead.")]
        public string IconUrl => _iconId != null ? CDN.GetTeamIconUrl(Id, _iconId, 128, ImageFormat.Jpeg) : null;
        /// <inheritdoc />
        public IReadOnlyList<ITeamMember> TeamMembers { get; private set; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public ulong OwnerUserId { get; private set; }

        private string _iconId;

        internal RestTeam(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestTeam Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestTeam(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal virtual void Update(Model model)
        {
            if (model.Icon.IsSpecified)
                _iconId = model.Icon.Value;
            Name = model.Name;
            OwnerUserId = model.OwnerUserId;
            TeamMembers = model.TeamMembers.Select(x => new RestTeamMember(Discord, x)).ToImmutableArray();
        }

        /// <inheritdoc />
        public string GetIconUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetTeamIconUrl(Id, _iconId, size, format);
    }
}
