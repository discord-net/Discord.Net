using Discord.Rest;
using System.Diagnostics;
using Model = Discord.API.VoiceRegion;

namespace Discord
{
    /// <summary>
    ///     Represents a REST-based voice region.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RestVoiceRegion : RestEntity<string>, IVoiceRegion
    {
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public bool IsVip { get; private set; }
        /// <inheritdoc />
        public bool IsOptimal { get; private set; }
        /// <inheritdoc />
        public bool IsDeprecated { get; private set; }
        /// <inheritdoc />
        public bool IsCustom { get; private set; }

        internal RestVoiceRegion(BaseDiscordClient client, string id)
            : base(client, id)
        {
        }
        internal static RestVoiceRegion Create(BaseDiscordClient client, Model model)
        {
            var entity = new RestVoiceRegion(client, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Name = model.Name;
            IsVip = model.IsVip;
            IsOptimal = model.IsOptimal;
            IsDeprecated = model.IsDeprecated;
            IsCustom = model.IsCustom;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsVip ? ", VIP" : "")}{(IsOptimal ? ", Optimal" : "")})";
    }
}
