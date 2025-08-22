using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Discord.Models.Json.Generator;

public sealed class SpecModel
{
    public string? Base { get; set; }

    //public Dictionary<string>
    [YamlMember(Alias = "props")] public Dictionary<string, SpecProperty> Properties { get; set; } = [];
}

public sealed class SpecProperty
{
    public required string Json { get; set; }

    public sealed class Serializer : INodeDeserializer, IYamlTypeConverter
    {
        public static readonly Serializer Instance = new();
        
        public bool Deserialize(
            IParser reader,
            Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer,
            out object? value,
            ObjectDeserializer rootDeserializer
        )
        {
            if (expectedType != typeof(SpecProperty))
            {
                value = null;
                return false;
            }
            
            switch (reader.Current)
            {
                case Scalar scalar:
                    value = new SpecProperty()
                    {
                        Json = scalar.Value
                    };
                    reader.MoveNext();
                    return true;
            }

            value = null;
            return false;
        }

        public bool Accepts(Type type)
            => type == typeof(SpecProperty);

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
            => rootDeserializer(type);

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            if (value is not SpecProperty {} specProperty) throw new InvalidOperationException();
            
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, specProperty.Json, ScalarStyle.DoubleQuoted, true, true));
        }
    }
}