using System.Text.Json;

namespace Discord.Serialization
{
    internal interface IJsonPropertyMap<TModel>
    {
        string Key { get; }

        void Write(TModel model, JsonWriter writer);
        void Read(TModel model, JsonReader reader);
    }
}
