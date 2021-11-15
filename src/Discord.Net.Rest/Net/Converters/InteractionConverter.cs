using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Discord.Net.Converters
{
    internal class InteractionConverter : JsonConverter
    {
        public static InteractionConverter Instance => new InteractionConverter();

        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType) => true;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var obj = JObject.Load(reader);
            var interaction = new API.Interaction();


            // Remove the data property for manual deserialization
            var result = obj.GetValue("data", StringComparison.OrdinalIgnoreCase);
            result?.Parent.Remove();

            // Populate the remaining properties.
            using (var subReader = obj.CreateReader())
            {
                serializer.Populate(subReader, interaction);
            }

            // Process the Result property
            if (result != null)
            {
                switch (interaction.Type)
                {
                    case InteractionType.ApplicationCommand:
                        {
                            var appCommandData = new API.ApplicationCommandInteractionData();
                            serializer.Populate(result.CreateReader(), appCommandData);
                            interaction.Data = appCommandData;
                        }
                        break;
                    case InteractionType.MessageComponent:
                        {
                            var messageComponent = new API.MessageComponentInteractionData();
                            serializer.Populate(result.CreateReader(), messageComponent);
                            interaction.Data = messageComponent;
                        }
                        break;
                    case InteractionType.ApplicationCommandAutocomplete:
                        {
                            var autocompleteData = new API.AutocompleteInteractionData();
                            serializer.Populate(result.CreateReader(), autocompleteData);
                            interaction.Data = autocompleteData;
                        }
                        break;
                }
            }
            else
                interaction.Data = Optional<IDiscordInteractionData>.Unspecified;

            return interaction;
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
