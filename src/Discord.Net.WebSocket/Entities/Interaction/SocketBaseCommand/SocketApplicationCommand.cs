using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.Gateway.ApplicationCommandCreatedUpdatedEvent;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represends a Websocket-based <see cref="IApplicationCommand"/>.
    /// </summary>
    public class SocketApplicationCommand : SocketEntity<ulong>, IApplicationCommand
    {
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
        public bool DefaultPermission { get; private set; }

        /// <summary>
        ///     A collection of <see cref="SocketApplicationCommandOption"/>'s for this command.
        /// </summary>
        /// <remarks>
        ///     If the <see cref="Type"/> is not a slash command, this field will be an empty collection.
        /// </remarks>
        public IReadOnlyCollection<SocketApplicationCommandOption> Options { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(this.Id);

        /// <summary>
        ///     Returns the guild this command resides in, if this command is a global command then it will return <see langword="null"/>
        /// </summary>
        public SocketGuild Guild
            => GuildId.HasValue ? Discord.GetGuild(this.GuildId.Value) : null;

        private ulong? GuildId { get; set; }

        internal SocketApplicationCommand(DiscordSocketClient client, ulong id, ulong? guildId)
            : base(client, id)
        {
            this.GuildId = guildId;
        }
        internal static SocketApplicationCommand Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketApplicationCommand(client, model.Id, model.GuildId.ToNullable());
            entity.Update(model);
            return entity;
        }

        internal void Update(API.ApplicationCommand model)
        {
            this.ApplicationId = model.ApplicationId;
            this.Description = model.Description;
            this.Name = model.Name;
            this.DefaultPermission = model.DefaultPermissions.GetValueOrDefault(true);

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => SocketApplicationCommandOption.Create(x)).ToImmutableArray()
                : new ImmutableArray<SocketApplicationCommandOption>();
        }

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => InteractionHelper.DeleteUnknownApplicationCommand(Discord, this.GuildId, this, options);

        /// <summary>
        ///     Modifies the current application command.
        /// </summary>
        /// <param name="func">The new properties to use when modifying the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when you pass in an invalid <see cref="ApplicationCommandProperties"/> type.</exception>
        public async Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null) where TArg : ApplicationCommandProperties
        {
            switch (typeof(TArg))
            {
                case Type messageCommand when messageCommand == typeof(MessageCommandProperties) && this.Type != ApplicationCommandType.Message:
                case Type slashCommand   when slashCommand   == typeof(SlashCommandProperties)   && this.Type != ApplicationCommandType.Slash:
                case Type userCommand    when userCommand    == typeof(UserCommandProperties)    && this.Type != ApplicationCommandType.User:
                    throw new InvalidOperationException($"Cannot modify this application command with the parameter type {nameof(TArg)}");
            }

            API.ApplicationCommand command = null;

            if (this.IsGlobalCommand)
            {
                command = await InteractionHelper.ModifyGlobalCommand(Discord, this, func, options).ConfigureAwait(false);
            }
            else
            {
                command = await InteractionHelper.ModifyGuildCommand(Discord, this, this.GuildId.Value, func, options);
            }

            this.Update(command);
        }

        // IApplicationCommand
        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;
        Task IApplicationCommand.ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options)
            => ModifyAsync(func, options);
    }
}
