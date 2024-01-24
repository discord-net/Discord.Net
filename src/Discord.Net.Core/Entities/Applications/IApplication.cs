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
        ///     Gets a collection of install parameters for this application; <see langword="null"/> if disabled.
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
        string TermsOfService { get; }
        /// <summary>
        ///     Gets the the url of the app's privacy policy.
        /// </summary>
        string PrivacyPolicy { get; }

        /// <summary>
        ///     Gets application's default custom authorization url. <see langword="null" /> if disabled.
        /// </summary>
        string CustomInstallUrl { get; }

        /// <summary>
        ///     Gets the application's role connection verification entry point. <see langword="null" /> if not set.
        /// </summary>
        string RoleConnectionsVerificationUrl { get; }

        /// <summary>
        ///     Gets the hex encoded key for verification in interactions.
        /// </summary>
        string VerifyKey { get; }

        /// <summary>
        ///     Gets the partial guild object of the application's developer's support server. <see langword="null" /> if not set.
        /// </summary>
        PartialGuild Guild { get; }

        /// <summary>
        ///     Gets the redirect uris configured for the application.
        /// </summary>
        IReadOnlyCollection<string> RedirectUris { get;}

        /// <summary>
        ///      Gets application's interactions endpoint url. <see langword="null" /> if not set.
        /// </summary>
        string InteractionsEndpointUrl { get; }

        /// <summary>
        ///     Gets the approximate count of the guild the application was added to. <see langword="null" /> if not returned.
        /// </summary>
        int? ApproximateGuildCount { get; }

        /// <summary>
        ///     Gets the application's discoverability state.
        /// </summary>
        ApplicationDiscoverabilityState DiscoverabilityState { get; }

        /// <summary>
        ///     Gets the application's discovery eligibility flags.
        /// </summary>
        DiscoveryEligibilityFlags DiscoveryEligibilityFlags { get; }

        /// <summary>
        ///     Gets the application's explicit content filter level for uploaded media content used in application commands.
        /// </summary>
        ApplicationExplicitContentFilterLevel ExplicitContentFilterLevel { get; }

        /// <summary>
        ///     Gets whether the bot is allowed to hook into the application's game directly.
        /// </summary>
        bool IsHook { get; }

        /// <summary>
        ///     Gets event types to be sent to the interaction endpoint.
        /// </summary>
        IReadOnlyCollection<string> InteractionEventTypes { get; }

        /// <summary>
        ///     Gets the interactions version application uses.
        /// </summary>
        ApplicationInteractionsVersion InteractionsVersion { get; }

        /// <summary>
        ///     Whether the application has premium subscriptions.
        /// </summary>
        bool IsMonetized { get; }

        /// <summary>
        ///     Gets the application's monetization eligibility flags.
        /// </summary>
        ApplicationMonetizationEligibilityFlags MonetizationEligibilityFlags { get; }

        /// <summary>
        ///     Gets the application's monetization state.
        /// </summary>
        ApplicationMonetizationState MonetizationState { get; }

        /// <summary>
        ///     Gets the application's rpc state.
        /// </summary>
        ApplicationRpcState RpcState { get; }

        /// <summary>
        ///     Gets the application's store state.
        /// </summary>
        ApplicationStoreState StoreState { get; }

        /// <summary>
        ///     Gets the application's verification state.
        /// </summary>
        ApplicationVerificationState VerificationState { get; }

        /// <summary>
        ///     Gets application install params configured for integration install types.
        /// </summary>
        IReadOnlyDictionary<ApplicationIntegrationType, ApplicationInstallParams> IntegrationTypesConfig { get; }
    }
}
