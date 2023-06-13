using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a Discord application created via the developer portal.
    /// </summary>
    public interface IApplication : ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the name of the application.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Gets the description of the application.
        /// </summary>
        string Description { get; }
        /// <summary>
        ///     Gets the RPC origins of the application.
        /// </summary>
        IReadOnlyCollection<string> RPCOrigins { get; }
        /// <summary>
        ///     Gets the application's public flags.
        /// </summary>
        ApplicationFlags Flags { get; }
        /// <summary>
        ///     Gets a collection of install parameters for this application.
        /// </summary>
        ApplicationInstallParams InstallParams { get; }
        /// <summary>
        ///     Gets a collection of tags related to the application.
        /// </summary>
        IReadOnlyCollection<string> Tags { get; }
        /// <summary>
        ///     Gets the icon URL of the application.
        /// </summary>
        string IconUrl { get; }
        /// <summary>
        ///     Gets if the bot is public. <see langword="null" /> if not set.
        /// </summary>
        bool? IsBotPublic { get; }
        /// <summary>
        ///     Gets if the bot requires code grant. <see langword="null" /> if not set.
        /// </summary>
        bool? BotRequiresCodeGrant { get; }
        /// <summary>
        ///     Gets the team associated with this application if there is one.
        /// </summary>
        ITeam Team { get; }
        /// <summary>
        ///     Gets the partial user object containing info on the owner of the application.
        /// </summary>
        IUser Owner { get; }
        /// <summary>
        ///     Gets the url of the app's terms of service.
        /// </summary>
        public string TermsOfService { get; }
        /// <summary>
        ///     Gets the the url of the app's privacy policy.
        /// </summary>
        public string PrivacyPolicy { get; }

        /// <summary>
        ///     Gets application's default custom authorization url. <see langword="null" /> if disabled.
        /// </summary>
        public string CustomInstallUrl { get; }

        /// <summary>
        ///     Gets the application's role connection verification entry point. <see langword="null" /> if not set.
        /// </summary>
        public string RoleConnectionsVerificationUrl { get; }

        /// <summary>
        ///     Gets the hex encoded key for verification in interactions.
        /// </summary>
        public string VerifyKey { get; }

        /// <summary>
        ///     Gets the partial guild object of the application's developer's support server. <see langword="null" /> if not set.
        /// </summary>
        public PartialGuild Guild { get; }

        /// <summary>
        ///     Gets the redirect uris configured for the application.
        /// </summary>
        public IReadOnlyCollection<string> RedirectUris { get;}

        /// <summary>
        ///      Gets application's interactions endpoint url. <see langword="null" /> if not set.
        /// </summary>
        public string InteractionsEndpointUrl { get; }

        /// <summary>
        ///     Gets the approximate count of the guild the application was added to. <see langword="null" /> if not returned.
        /// </summary>
        public int? ApproximateGuildCount { get; }
    }
}
