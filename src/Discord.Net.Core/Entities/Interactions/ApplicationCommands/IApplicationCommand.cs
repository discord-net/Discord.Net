using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     The base command model that belongs to an application.
    /// </summary>
    public interface IApplicationCommand : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     Gets the unique id of the parent application.
        /// </summary>
        ulong ApplicationId { get; }

        /// <summary>
        ///     Gets the type of the command.
        /// </summary>
        ApplicationCommandType Type { get; }

        /// <summary>
        ///     Gets the name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the description of the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets whether the command is enabled by default when the app is added to a guild.
        /// </summary>
        bool IsDefaultPermission { get; }

        /// <summary>
        ///     Indicates whether the command is available in DMs with the app.
        /// </summary>
        /// <remarks>
        ///     Only for globally-scoped commands.
        /// </remarks>
        [Obsolete("This property will be deprecated soon. Use ContextTypes instead.")]
        bool IsEnabledInDm { get; }

        /// <summary>
        ///     Indicates whether the command is age restricted.
        /// </summary>
        bool IsNsfw { get; }

        /// <summary>
        ///     Set of default <see cref="GuildPermission"/> required to invoke the command.
        /// </summary>
        GuildPermissions DefaultMemberPermissions { get; }

        /// <summary>
        ///     Gets a collection of options for this application command.
        /// </summary>
        IReadOnlyCollection<IApplicationCommandOption> Options { get; }

        /// <summary>
        ///     Gets the localization dictionary for the name field of this command.
        /// </summary>
        IReadOnlyDictionary<string, string> NameLocalizations { get; }

        /// <summary>
        ///     Gets the localization dictionary for the description field of this command.
        /// </summary>
        IReadOnlyDictionary<string, string> DescriptionLocalizations { get; }

        /// <summary>
        ///     Gets the localized name of this command.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        string NameLocalized { get; }

        /// <summary>
        ///     Gets the localized description of this command.
        /// </summary>
        /// <remarks>
        ///     Only returned when the `withLocalizations` query parameter is set to <see langword="false"/> when requesting the command.
        /// </remarks>
        string DescriptionLocalized { get; }

        /// <summary>
        ///     Gets context types the command can be used in; <see langword="null" /> if not specified.
        /// </summary>
        IReadOnlyCollection<InteractionContextType> ContextTypes { get; }

        /// <summary>
        ///     Gets the install method for the command; <see langword="null" /> if not specified.
        /// </summary>
        IReadOnlyCollection<ApplicationIntegrationType> IntegrationTypes { get; }

        /// <summary>
        ///     Modifies the current application command.
        /// </summary>
        /// <param name="func">The new properties to use when modifying the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Modifies the current application command.
        /// </summary>
        /// <param name="func">The new properties to use when modifying the command.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when you pass in an invalid <see cref="ApplicationCommandProperties"/> type.</exception>
        Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null)
            where TArg : ApplicationCommandProperties;
    }
}
