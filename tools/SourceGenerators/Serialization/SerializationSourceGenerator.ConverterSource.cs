using Microsoft.CodeAnalysis;

namespace Discord.Net.SourceGenerators.Serialization
{
    public partial class SerializationSourceGenerator
    {
        private static string GenerateConverter(INamedTypeSymbol @class)
        {
return $@"
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Net.Serialization.Converters
{{
    public class {@class.Name}Converter : JsonConverter<{@class.ToDisplayString()}>
    {{
        public override {@class.ToDisplayString()} Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {{
            return default;
        }}

        public override void Write(
            Utf8JsonWriter writer,
            {@class.ToDisplayString()} value,
            JsonSerializerOptions options)
        {{
            writer.WriteNull();
        }}
    }}
}}";
        }
    }
}
