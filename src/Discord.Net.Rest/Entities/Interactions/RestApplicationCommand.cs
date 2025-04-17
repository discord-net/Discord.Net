using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of the <see cref="IApplicationCommand"/>.
    /// </summary>
    public abstract class RestApplicationCommand : RestEntity<ulong>, IApplicationCommand
    {
        /// <inheritdoc/>
        public ulong ApplicationId { get; private set; }

        /// <inheritdoc/>
        public ApplicationCommandType Type { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

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
        ///     Gets a collection of options for this command.
        /// </summary>
        public IReadOnlyCollection<RestApplicationCommandOption> Options { get; private set; }

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

        internal RestApplicationCommand(BaseDiscordClient client, ulong id)
            : base(client, id) { }

        internal static RestApplicationCommand Create(BaseDiscordClient client, Model model, ulong? guildId)
        {
            return guildId.HasValue
                ? RestGuildCommand.Create(client, model, guildId.Value)
                : RestGlobalCommand.Create(client, model);
        }

        internal virtual void Update(Model model)
        {
            Type = model.Type;
            ApplicationId = model.ApplicationId;
            Name = model.Name;
            Description = model.Description;
            IsDefaultPermission = model.DefaultPermissions.GetValueOrDefault(true);

            Options = model.Options.IsSpecified
                ? model.Options.Value.Select(RestApplicationCommandOption.Create).ToImmutableArray()
                : ImmutableArray.Create<RestApplicationCommandOption>();

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
        public abstract Task DeleteAsync(RequestOptions options = null);

        /// <inheritdoc />
        public Task ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options = null)
        {
            return ModifyAsync<ApplicationCommandProperties>(func, options);
        }

        /// <inheritdoc/>
        public abstract Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null)
            where TArg : ApplicationCommandProperties;

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommand.Options => Options;
    }
}
