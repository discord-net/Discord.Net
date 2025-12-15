using Newtonsoft.Json;

namespace Discord.API
{
    internal class UnknownComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public int RawType { get; set; }

        public ComponentType Type => (ComponentType)RawType;

        [JsonProperty("id")]
        public Optional<int> Id { get; set; }

        int? IMessageComponent.Id => Id.ToNullable();

        public string RawJson { get; set; }

        public UnknownComponent() { }

        /// <inheritdoc />
        IMessageComponentBuilder IMessageComponent.ToBuilder()
            => throw new System.NotSupportedException("Unknown components cannot be converted to builders.");
    }
}
