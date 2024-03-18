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
        ///    Gets whether or not this command is a global application command.
        /// </summary>
        public bool IsGlobalCommand
            => GuildId is null;

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

        /// <inheritdoc/>
        [Obsolete("This property will be deprecated soon. Use ContextTypes instead.")]
        public bool IsEnabledInDm { get; private set; }

        /// <inheritdoc/>
        public bool IsNsfw { get; private set; }

        /// <inheritdoc/>
        public GuildPermissions DefaultMemberPermissions { get; private set; }

        /// <summary>
        ///     Gets a collection of <see cref="SocketApplicationCommandOption"/>s for this command.
        /// </summary>
        /// <remarks>
        ///     If the <see cref="Type"/> is not a slash command, this field will be an empty collection.
        /// </remarks>
        public IReadOnlyCollection<SocketApplicationCommandOption> Options { get; private set; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> NameLocalizations { get; private set; }

        /// <summary>
        ///     Gets the localization dictionary for the description field of this command.
        /// </summary>
        public IReadOnlyDictionary<string, string> DescriptionLocalizations { get; private set; }

        /// <summary>
        ///     Gets the localized name of this command.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        public string NameLocalized { get; private set; }

        /// <summary>
        ///     Gets the localized description of this command.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        public string DescriptionLocalized { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ApplicationIntegrationType> IntegrationTypes { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<InteractionContextType> ContextTypes { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        /// <summary>
        ///     Gets the guild this command resides in; if this command is a global command then it will return <see langword="null"/>
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

            NameLocalizations = model.NameLocalizations.GetValueOrDefault(null)?.ToImmutableDictionary() ??
                                ImmutableDictionary<string, string>.Empty;

            DescriptionLocalizations = model.DescriptionLocalizations.GetValueOrDefault(null)?.ToImmutableDictionary() ??
                                       ImmutableDictionary<string, string>.Empty;

            NameLocalized = model.NameLocalized.GetValueOrDefault();
            DescriptionLocalized = model.DescriptionLocalized.GetValueOrDefault();

#pragma warning disable CS0618 // Type or member is obsolete
            IsEnabledInDm = model.DmPermission.GetValueOrDefault(true).GetValueOrDefault(true);
#pragma warning restore CS0618 // Type or member is obsolete
            DefaultMemberPermissions = new GuildPermissions((ulong)model.DefaultMemberPermission.GetValueOrDefault(0).GetValueOrDefault(0));
            IsNsfw = model.Nsfw.GetValueOrDefault(false).GetValueOrDefault(false);

            IntegrationTypes = model.IntegrationTypes.GetValueOrDefault(null)?.ToImmutableArray();
            ContextTypes = model.ContextTypes.GetValueOrDefault(null)?.ToImmutableArray();
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
