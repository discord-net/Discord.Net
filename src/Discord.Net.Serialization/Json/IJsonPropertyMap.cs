using System.Text.Json;
using System.Text.Utf8;

namespace Discord.Serialization
{
    internal interface IJsonPropertyMap<TModel>
    {
        string Utf16Key { get; }
        Utf8String Utf8Key { get; }

        void Write(TModel model, ref JsonWriter writer);
        void Read(TModel model, ref JsonReader reader);
    }
}
