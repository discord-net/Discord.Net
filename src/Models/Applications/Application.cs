using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord application object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/application#application-object-application-structure"/>
    /// </remarks>
    public record Application
    {
        /// <summary>
        /// The id of the <see cref="Application"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// The name of the <see cref="Application"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// The icon hash of the <see cref="Application"/>.
        /// </summary>
        [JsonPropertyName("icon")]
        public string? Icon { get; init; }

        /// <summary>
        /// The description of the <see cref="Application"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; } // Required property candidate

        /// <summary>
        /// An array of rpc origin urls, if rpc is enabled.
        /// </summary>
        [JsonPropertyName("rpc_origins")]
        public Optional<string[]> RpcOrigins { get; init; }

        /// <summary>
        /// When false only app owner can join the <see cref="Application"/>'s bot to <see cref="Guild"/>s.
        /// </summary>
        [JsonPropertyName("bot_public")]
        public bool BotPublic { get; init; }

        /// <summary>
        /// When true the app's bot will only join upon completion of the full oauth2 code grant flow.
        /// </summary>
        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequireCodeGrant { get; init; }

        /// <summary>
        /// The url of the <see cref="Application"/>'s terms of service.
        /// </summary>
        [JsonPropertyName("terms_of_service_url")]
        public Optional<string> TermsOfServiceUrl { get; init; }

        /// <summary>
        /// The url of the <see cref="Application"/>'s privacy policy.
        /// </summary>
        [JsonPropertyName("privacy_policy_url")]
        public Optional<string> PrivacyPolicyUrl { get; init; }

        /// <summary>
        /// Partial <see cref="User"/> containing info on the owner of the <see cref="Application"/>.
        /// </summary>
        [JsonPropertyName("owner")]
        public User? Owner { get; init; } // Required property candidate

        /// <summary>
        /// If this <see cref="Application"/> is a game sold on Discord, this field will be
        /// the summary field for the store page of its primary sku.
        /// </summary>
        [JsonPropertyName("summary")]
        public string? Summary { get; init; } // Required property candidate

        /// <summary>
        /// The hex encoded key for verification in interactions and the GameSDK's GetTicket.
        /// </summary>
        [JsonPropertyName("verify_key")]
        public string? VerifyKey { get; init; } // Required property candidate

        /// <summary>
        /// If the <see cref="Application"/> belongs to a <see cref="Models.Team"/>,
        /// this will be a list of the <see cref="TeamMember"/>s of that <see cref="Models.Team"/>.
        /// </summary>
        [JsonPropertyName("team")]
        public Team? Team { get; init; }

        /// <summary>
        /// If this <see cref="Application"/> is a game sold on Discord, this field will
        /// be the <see cref="Guild"/> to which it has been linked.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; init; }

        /// <summary>
        /// If this <see cref="Application"/> is a game sold on Discord, this field will
        /// be the id of the "Game SKU" that is created, if exists.
        /// </summary>
        [JsonPropertyName("primary_sku_id")]
        public Optional<Snowflake> PrimarySkuId { get; init; }

        /// <summary>
        /// If this <see cref="Application"/> is a game sold on Discord, this field will
        /// be the URL slug that links to the store page.
        /// </summary>
        [JsonPropertyName("slug")]
        public Optional<string> Slug { get; init; }

        /// <summary>
        /// The <see cref="Application"/>'s default rich presence invite cover image hash.
        /// </summary>
        [JsonPropertyName("cover_image")]
        public Optional<string> CoverImage { get; init; }

        /// <summary>
        /// The <see cref="Application"/>'s public flags.
        /// </summary>
        [JsonPropertyName("flags")]
        public ApplicationFlags Flags { get; init; }
    }
}
