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
        ///     Gets if the bot is public.
        /// </summary>
        bool IsBotPublic { get; }
        /// <summary>
        ///     Gets if the bot requires code grant.
        /// </summary>
        bool BotRequiresCodeGrant { get; }
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

    }
}
