using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using GatewayModel = Discord.API.Gateway.ApplicationCommandCreatedUpdatedEvent;
using Model = Discord.API.ApplicationCommand;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a Websocket-based <see cref="IApplicationCommand"/>.
    /// </summary>
    public class SocketApplicationCommand : SocketEntity<ulong>, IApplicationCommand
    {
        #region SocketApplicationCommand
        /// <summary>
        ///    <see langword="true"/> if this command is a global command, otherwise <see langword="false"/>.
        /// </summary>
        public bool IsGlobalCommand
            => Guild == null;

        /// <inheritdoc/>
        public ulong ApplicationId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public ApplicationCommandType Type { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public bool IsDefaultPermission { get; private set; }

        /// <summary>
        ///     A collection of <see cref="SocketApplicationCommandOption"/>'s for this command.
        /// </summary>
        /// <remarks>
        ///     If the <see cref="Type"/> is not a slash command, this field will be an empty collection.
        /// </remarks>
        public IReadOnlyCollection<SocketApplicationCommandOption> Options { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        /// <summary>
        ///     Returns the guild this command resides in, if this command is a global command then it will return <see langword="null"/>
        /// </summary>
        public SocketGuild Guild
            => GuildId.HasValue ? Discord.GetGuild(GuildId.Value) : null;

        private ulong? GuildId { get; set; }

        internal SocketApplicationCommand(DiscordSocketClient client, ulong id, ulong? guildId)
            : base(client, id)
        {
            GuildId = guildId;
        }
        internal static SocketApplicationCommand Create(DiscordSocketClient client, GatewayModel model)
        {
            var entity = new SocketApplicationCommand(client, model.Id, model.GuildId.ToNullable());
            entity.Update(model);
            return entity;
        }

        internal static SocketApplicationCommand Create(DiscordSocketClient client, Model model, ulong? guildId = null)
        {
            var entity = new SocketApplicationCommand(client, model.Id, guildId);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            ApplicationId = model.ApplicationId;
            Description = model.Description;
            Name = model.Name;
            IsDefaultPermission = model.DefaultPermissions.GetValueOrDefault(true);
            Type = model.Type;

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(SocketApplicationCommandOption.Create).ToImmutableArray()
                : ImmutableArray.Create<SocketApplicationCommandOption>();
        }

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => InteractionHelper.DeleteUnknownApplicationCommandAsync(Discord, GuildId, this, options);

        /// <inheritdoc />
        public Task ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options = null)
        {
            return ModifyAsync<ApplicationCommandProperties>(func, options);
        }

        /// <inheritdoc />
        public async Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            var command = IsGlobalCommand
                ? await InteractionHelper.ModifyGlobalCommandAsync(Discord, this, func, options).ConfigureAwait(false)
                : await InteractionHelper.ModifyGuildCommandAsync(Discord, this, GuildId.Value, func, options);

            Update(command);
        }
        #endregion

        #region  IApplicationCommand
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;
        #endregion
    }
}
