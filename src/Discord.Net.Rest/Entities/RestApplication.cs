using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Application;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based entity that contains information about a Discord application created via the developer portal.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestApplication : RestEntity<ulong>, IApplication
    {
        protected string _iconId;

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public string Description { get; private set; }
        /// <inheritdoc />
        public string[] RPCOrigins { get; private set; }
        /// <inheritdoc />
        public ulong Flags { get; private set; }

        /// <inheritdoc />
        public IUser Owner { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => CDN.GetApplicationIconUrl(Id, _iconId);

        internal RestApplication(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestApplication Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestApplication(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {            
            Description = model.Description;
            RPCOrigins = model.RPCOrigins;
            Name = model.Name;
            _iconId = model.Icon;

            if (model.Flags.IsSpecified)
                Flags = model.Flags.Value; //TODO: Do we still need this?
            if (model.Owner.IsSpecified)
                Owner = RestUser.Create(Discord, model.Owner.Value);
        }

        /// <exception cref="InvalidOperationException">Unable to update this object from a different application token.</exception>
        public async Task UpdateAsync()
        {
            var response = await Discord.ApiClient.GetMyApplicationAsync().ConfigureAwait(false);
            if (response.Id != Id)
                throw new InvalidOperationException("Unable to update this object from a different application token.");
            Update(response);
        }

        /// <summary>
        ///     Gets the name of the application.
        /// </summary>
        /// <returns>
        ///     Name of the application.
        /// </returns>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";
    }
}
