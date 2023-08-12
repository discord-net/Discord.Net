using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Model = Discord.API.Connection;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestConnection : IConnection
    {
        /// <inheritdoc />
        public string Id { get; private set; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public string Type { get; private set; }
        /// <inheritdoc />
        public bool? IsRevoked { get; private set; }
        /// <inheritdoc />
        public IReadOnlyCollection<IIntegration> Integrations { get; private set; }
        /// <inheritdoc />
        public bool Verified { get; private set; }
        /// <inheritdoc />
        public bool FriendSync { get; private set; }
        /// <inheritdoc />
        public bool ShowActivity { get; private set; }
        /// <inheritdoc />
        public ConnectionVisibility Visibility { get; private set; }

        internal BaseDiscordClient Discord { get; }

        internal RestConnection(BaseDiscordClient discord)
        {
            Discord = discord;
        }

        internal static RestConnection Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestConnection(discord);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            Type = model.Type;
            IsRevoked = model.Revoked.IsSpecified ? model.Revoked.Value : null;
            Integrations = model.Integrations.IsSpecified ? model.Integrations.Value
                .Select(intergration => RestIntegration.Create(Discord, null, intergration)).ToImmutableArray() : null;
            Verified = model.Verified;
            FriendSync = model.FriendSync;
            ShowActivity = model.ShowActivity;
            Visibility = model.Visibility;
        }

        /// <summary>
        ///     Gets the name of the connection.
        /// </summary>
        /// <returns>
        ///     Name of the connection.
        /// </returns>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, {Type}{(IsRevoked.GetValueOrDefault() ? ", Revoked" : "")})";
    }
}
