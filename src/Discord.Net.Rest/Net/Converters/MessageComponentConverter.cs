using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Discord.Net.Converters
{
    internal class MessageComponentConverter : JsonConverter
    {
        public static MessageComponentConverter Instance => new MessageComponentConverter();

        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType) => true;
        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var messageComponent = default(IMessageComponent);
            switch ((ComponentType)jsonObject["type"].Value<int>())
            {
                case ComponentType.ActionRow:
                    messageComponent = new API.ActionRowComponent();
                    break;
                case ComponentType.Button:
                    messageComponent = new API.ButtonComponent();
                    break;
                case ComponentType.SelectMenu:
                    messageComponent = new API.SelectMenuComponent();
                    break;
            }
            serializer.Populate(jsonObject.CreateReader(), messageComponent);
            return messageComponent;
        }
    }
}
