using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.ApplicationCommandCreatedUpdatedEvent;

namespace Discord.WebSocket
{
    public class SocketApplicationCommand : SocketEntity<ulong>, IApplicationCommand
    {
        public ulong ApplicationId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IReadOnlyCollection<SocketApplicationCommandOption> Options { get; private set; }

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        public SocketGuild Guild
            => Discord.GetGuild(this.GuildId);
        private ulong GuildId { get; set; }

        internal SocketApplicationCommand(DiscordSocketClient client, ulong id)
            : base(client, id)
        {

        }
        internal static SocketApplicationCommand Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketApplicationCommand(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            this.ApplicationId = model.ApplicationId;
            this.Description = model.Description;
            this.Name = model.Name;
            this.GuildId = model.GuildId;

            this.Options = model.Options.Any()
                ? model.Options.Select(x => SocketApplicationCommandOption.Create(x)).ToImmutableArray()
                : new ImmutableArray<SocketApplicationCommandOption>();
        }

        public Task DeleteAsync(RequestOptions options = null) => throw new NotImplementedException();
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;
    }
}
