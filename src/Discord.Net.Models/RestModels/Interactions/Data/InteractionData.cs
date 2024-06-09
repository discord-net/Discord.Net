using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(InteractionDataConverter))]
public abstract class InteractionData
{}
