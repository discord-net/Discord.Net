using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an application object.
    /// </summary>
    public record Application
    {
        /// <summary>
        ///     Creates a <see cref="Application"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of the app.</param>
        /// <param name="name">The name of the app.</param>
        /// <param name="icon">The icon hash of the app.</param>
        /// <param name="description">The description of the app.</param>
        /// <param name="rpcOrigins">An array of rpc origin urls, if rpc is enabled.</param>
        /// <param name="botPublic">When false only app owner can join the app's bot to guilds.</param>
        /// <param name="botRequireCodeGrant">When true the app's bot will only join upon completion of the full oauth2 code grant flow.</param>
        /// <param name="termsOfServiceUrl">The url of the app's terms of service.</param>
        /// <param name="privacyPolicyUrl">The url of the app's privacy policy.</param>
        /// <param name="owner">Partial user object containing info on the owner of the application.</param>
        /// <param name="summary">If this application is a game sold on Discord, this field will be the summary field for the store page of its primary sku.</param>
        /// <param name="verifyKey">The hex encoded key for verification in interactions and the GameSDK's GetTicket.</param>
        /// <param name="team">If the application belongs to a team, this will be a list of the members of that team.</param>
        /// <param name="guildId">If this application is a game sold on Discord, this field will be the guild to which it has been linked.</param>
        /// <param name="primarySkuId">If this application is a game sold on Discord, this field will be the id of the "Game SKU" that is created, if exists.</param>
        /// <param name="slug">If this application is a game sold on Discord, this field will be the URL slug that links to the store page.</param>
        /// <param name="coverImage">The application's default rich presence invite cover image hash.</param>
        /// <param name="flags">The application's public flags.</param>
        [JsonConstructor]
        public Application(Snowflake id, string name, string? icon, string description, Optional<string[]> rpcOrigins, bool botPublic, bool botRequireCodeGrant, Optional<string> termsOfServiceUrl, Optional<string> privacyPolicyUrl, User owner, string summary, string verifyKey, Team? team, Optional<Snowflake> guildId, Optional<Snowflake> primarySkuId, Optional<string> slug, Optional<string> coverImage, ApplicationFlags flags)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Description = description;
            RpcOrigins = rpcOrigins;
            BotPublic = botPublic;
            BotRequireCodeGrant = botRequireCodeGrant;
            TermsOfServiceUrl = termsOfServiceUrl;
            PrivacyPolicyUrl = privacyPolicyUrl;
            Owner = owner;
            Summary = summary;
            VerifyKey = verifyKey;
            Team = team;
            GuildId = guildId;
            PrimarySkuId = primarySkuId;
            Slug = slug;
            CoverImage = coverImage;
            Flags = flags;

        }

        /// <summary>
        ///     The id of the app.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The name of the app.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     The icon hash of the app.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; }

        /// <summary>
        ///     The description of the app.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; }

        /// <summary>
        ///     An array of rpc origin urls, if rpc is enabled.
        /// </summary>
        [JsonPropertyName("rpc_origins")]
        public Optional<string[]> RpcOrigins { get; }

        /// <summary>
        ///     When false only app owner can join the app's bot to guilds.
        /// </summary>
        [JsonPropertyName("bot_public")]
        public bool BotPublic { get; }

        /// <summary>
        ///     When true the app's bot will only join upon completion of the full oauth2 code grant flow.
        /// </summary>
        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequireCodeGrant { get; }

        /// <summary>
        ///     The url of the app's terms of service.
        /// </summary>
        [JsonPropertyName("terms_of_service_url")]
        public Optional<string> TermsOfServiceUrl { get; }

        /// <summary>
        ///     The url of the app's privacy policy.
        /// </summary>
        [JsonPropertyName("privacy_policy_url")]
        public Optional<string> PrivacyPolicyUrl { get; }

        /// <summary>
        ///     Partial user object containing info on the owner of the application.
        /// </summary>
        [JsonPropertyName("owner")]
        public User Owner { get; }

        /// <summary>
        ///     If this application is a game sold on Discord, this field will be the summary field for the store page of its primary sku.
        /// </summary>
        [JsonPropertyName("summary")]
        public string Summary { get; }

        /// <summary>
        ///     The hex encoded key for verification in interactions and the GameSDK's GetTicket.
        /// </summary>
        [JsonPropertyName("verify_key")]
        public string VerifyKey { get; }

        /// <summary>
        ///     If the application belongs to a team, this will be a list of the members of that team.
        /// </summary>
        [JsonPropertyName("team")]
        public Team? Team { get; }

        /// <summary>
        ///     If this application is a game sold on Discord, this field will be the guild to which it has been linked.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; }

        /// <summary>
        ///     If this application is a game sold on Discord, this field will be the id of the "Game SKU" that is created, if exists.
        /// </summary>
        [JsonPropertyName("primary_sku_id")]
        public Optional<Snowflake> PrimarySkuId { get; }

        /// <summary>
        ///     If this application is a game sold on Discord, this field will be the URL slug that links to the store page.
        /// </summary>
        [JsonPropertyName("slug")]
        public Optional<string> Slug { get; }

        /// <summary>
        ///     The application's default rich presence invite cover image hash.
        /// </summary>
        [JsonPropertyName("cover_image")]
        public Optional<string> CoverImage { get; }

        /// <summary>
        ///     The application's public flags.
        /// </summary>
        [JsonPropertyName("flags")]
        public ApplicationFlags Flags { get; }
    }
}
