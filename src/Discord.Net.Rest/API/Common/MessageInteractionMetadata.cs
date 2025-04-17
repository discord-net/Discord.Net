using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API;

internal class MessageInteractionMetadata
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("type")]
    public InteractionType Type { get; set; }
    
    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("authorizing_integration_owners")]
    public Dictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; set; }

    [JsonProperty("original_response_message_id")]
    public Optional<ulong> OriginalResponseMessageId { get; set; }

    [JsonProperty("name")]
    public Optional<string> Name { get; set; }

    [JsonProperty("interacted_message_id")]
    public Optional<ulong> InteractedMessageId { get; set; }

    [JsonProperty("triggering_interaction_metadata")]
    public Optional<MessageInteractionMetadata> TriggeringInteractionMetadata { get; set; }
}
