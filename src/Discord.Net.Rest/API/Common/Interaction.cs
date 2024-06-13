using Newtonsoft.Json;

using System.Collections.Generic;

namespace Discord.API;

[JsonConverter(typeof(Net.Converters.InteractionConverter))]
internal class Interaction
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonProperty("type")]
    public InteractionType Type { get; set; }

    [JsonProperty("data")]
    public Optional<IDiscordInteractionData> Data { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonProperty("channel")]
    public Optional<Channel> Channel { get; set; }

    [JsonProperty("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonProperty("member")]
    public Optional<GuildMember> Member { get; set; }

    [JsonProperty("user")]
    public Optional<User> User { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("version")]
    public int Version { get; set; }

    [JsonProperty("message")]
    public Optional<Message> Message { get; set; }

    [JsonProperty("locale")]
    public Optional<string> UserLocale { get; set; }

    [JsonProperty("guild_locale")]
    public Optional<string> GuildLocale { get; set; }

    [JsonProperty("entitlements")]
    public Entitlement[] Entitlements { get; set; }

    [JsonProperty("authorizing_integration_owners")]
    public Dictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; set; }

    [JsonProperty("context")]
    public Optional<InteractionContextType> ContextType { get; set; }

    [JsonProperty("app_permissions")]
    public GuildPermission ApplicationPermissions { get; set; }
}
