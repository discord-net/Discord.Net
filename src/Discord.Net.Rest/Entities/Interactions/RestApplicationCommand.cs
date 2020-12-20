using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a rest implementation of the <see cref="IApplicationCommand"/>
    /// </summary>
    public abstract class RestApplicationCommand : RestEntity<ulong>, IApplicationCommand
    {
        public ulong ApplicationId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IReadOnlyCollection<RestApplicationCommandOption> Options { get; private set; }

        public RestApplicationCommandType CommandType { get; internal set; }

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        internal RestApplicationCommand(BaseDiscordClient client, ulong id)
            : base(client, id)
        {

        }

        internal static RestApplicationCommand Create(BaseDiscordClient client, Model model, RestApplicationCommandType type, ulong guildId = 0)
        {
            if (type == RestApplicationCommandType.GlobalCommand)
                return RestGlobalCommand.Create(client, model);

            if (type == RestApplicationCommandType.GuildCommand)
                return RestGuildCommand.Create(client, model, guildId);

            return null;
        }

        internal virtual void Update(Model model)
        {
            this.ApplicationId = model.ApplicationId;
            this.Name = model.Name;
            this.Description = model.Description;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => RestApplicationCommandOption.Create(x)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;

        public virtual Task DeleteAsync(RequestOptions options = null) => throw new NotImplementedException();
    }
}
