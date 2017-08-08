using System;
using System.Text.Json;

namespace Discord.Serialization
{
    internal interface IJsonPropertyMap<TModel>
    {
        string Key { get; }
        ReadOnlyBuffer<byte> Utf8Key { get; }

        void Write(TModel model, ref JsonWriter writer);
        void Read(TModel model, ref JsonReader reader);
    }
}
