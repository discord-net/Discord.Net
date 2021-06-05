using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Discord.Net.SourceGenerators.Serialization.Utils;

namespace Discord.Net.SourceGenerators.Serialization
{
    internal record SerializedType(
        INamedTypeSymbol Declaration)
    {
        public virtual string ConverterTypeName
            => $"{Declaration.Name}Converter";

        protected virtual IEnumerable<IPropertySymbol> SymbolsToSerialize
            => Declaration.GetProperties(includeInherited: true)
                .Where(x => !x.IsReadOnly);

        public virtual string GenerateSourceCode(string outputNamespace)
        {
            var deserializers = SymbolsToSerialize
                .Select(GenerateFieldReader);

            var bytes = string.Join("\n",
                deserializers.Select(x => x.utf8));
            var fields = string.Join("\n",
                deserializers.Select(x => x.field));
            var readers = string.Join("\n",
                deserializers.Select(x => x.reader));

            var fieldUnassigned = string.Join("\n                || ",
                deserializers
                    .Where(x => x.type.NullableAnnotation != NullableAnnotation.Annotated)
                    .Select(
                        x => $"{x.snakeCase}OrDefault is not {x.type} {x.snakeCase}"));

            var constructorParams = string.Join(",\n",
                deserializers
                    .Select(x => $"                {x.name}: {x.snakeCase}{(x.type.NullableAnnotation == NullableAnnotation.Annotated ? "OrDefault" : "")}"));

return $@"using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace {outputNamespace}.Converters
{{
    internal class {ConverterTypeName} : JsonConverter<{Declaration.ToDisplayString()}>
    {{
{bytes}

        public override {Declaration.ToDisplayString()}? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {{
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException(""Expected StartObject"");

{fields}

            while (reader.Read())
            {{
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException(""Expected PropertyName"");

{readers}
                else if (!reader.Read())
                    throw new JsonException();

                if (reader.TokenType == JsonTokenType.StartArray
                    || reader.TokenType == JsonTokenType.StartObject)
                    reader.Skip();
            }}

            if ({fieldUnassigned})
                throw new JsonException(""Missing field"");

            return new {Declaration.ToDisplayString()}(
{constructorParams}
            );
        }}

        public override void Write(
            Utf8JsonWriter writer,
            {Declaration.ToDisplayString()} value,
            JsonSerializerOptions options)
        {{
            writer.WriteNullValue();
        }}
    }}
}}";

            static (string name, ITypeSymbol type, string snakeCase, string utf8, string field, string reader)
                GenerateFieldReader(IPropertySymbol member, int position)
            {
                var needsNullableAnnotation = false;
                if (member.Type.IsValueType
                    && member.Type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
                    needsNullableAnnotation = true;

                var snakeCase = ConvertToSnakeCase(member.Name);
                return (member.Name, member.Type, snakeCase,
$@"        private static ReadOnlySpan<byte> {member.Name}Bytes => new byte[]
        {{
            // {snakeCase}
            {string.Join(", ", Encoding.UTF8.GetBytes(snakeCase))}
        }};",
$"            {member.Type.WithNullableAnnotation(NullableAnnotation.Annotated).ToDisplayString()}{(needsNullableAnnotation ? "?" : "")} {snakeCase}OrDefault = default;",
$@"                {(position > 0 ? "else " : "")}if (reader.ValueTextEquals({member.Name}Bytes))
                {{
                    if (!reader.Read())
                        throw new JsonException(""Expected value"");

                    var cvt = options.GetConverter(
                        typeof({member.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString()}));

                    if (cvt is JsonConverter<{member.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString()}> converter)
                        {snakeCase}OrDefault = converter.Read(ref reader,
                            typeof({member.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString()}),
                            options);
                    else
                        {snakeCase}OrDefault = JsonSerializer.Deserialize<{member.Type.ToDisplayString()}>(
                            ref reader, options);
                }}");
            }
        }
    }

    internal record DiscriminatedUnionSerializedType(
        INamedTypeSymbol Declaration,
        ISymbol Discriminator)
        : SerializedType(Declaration)
    {
        public List<DiscriminatedUnionMemberSerializedType> Members { get; }
            = new();

        public override string GenerateSourceCode(string outputNamespace)
        {
            var discriminatorField = ConvertToSnakeCase(Discriminator.Name);

            var discriminatorType = Discriminator switch
            {
                IPropertySymbol prop => prop.Type,
                IFieldSymbol field => field.Type,
                _ => throw new InvalidOperationException(
                    "Unsupported discriminator member type")
            };

            var switchCaseMembers = string.Join(",\n",
                Members.Select(
                    x => $@"                {x.DiscriminatorValue.ToCSharpString()}
                    => JsonSerializer.Deserialize(ref copy,
                        typeof({x.Declaration.ToDisplayString()}), options)"));

            return $@"using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace {outputNamespace}.Internal.Converters
{{
    internal class {ConverterTypeName} : JsonConverter<{Declaration.ToDisplayString()}>
    {{
        private static ReadOnlySpan<byte> DiscriminatorBytes => new byte[]
        {{
            // {discriminatorField}
            {string.Join(", ", Encoding.UTF8.GetBytes(discriminatorField))}
        }};

        public override {Declaration.ToDisplayString()}? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {{
            var copy = reader;
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException(""Expected StartObject"");

            {discriminatorType.ToDisplayString()}? discriminator = null;

            while (reader.Read())
            {{
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException(""Expected PropertyName"");

                if (reader.ValueTextEquals(DiscriminatorBytes))
                {{
                    if (!reader.Read())
                        throw new JsonException(""Expected value"");

                    var cvt = options.GetConverter(
                        typeof({discriminatorType.ToDisplayString()}));

                    if (cvt is JsonConverter<{discriminatorType.ToDisplayString()}> converter)
                        discriminator = converter.Read(ref reader,
                            typeof({discriminatorType.ToDisplayString()}),
                            options);
                    else
                        discriminator = JsonSerializer
                            .Deserialize<{discriminatorType.ToDisplayString()}>(
                                ref reader, options);
                }}
                else if (!reader.Read())
                    throw new JsonException(""Expected value"");

                if (reader.TokenType == JsonTokenType.StartArray
                    || reader.TokenType == JsonTokenType.StartObject)
                    reader.Skip();
            }}

            var result = discriminator switch
            {{
{switchCaseMembers},
                _ => throw new JsonException(""Unknown discriminator value"")
            }} as {Declaration.ToDisplayString()};

            reader = copy;
            return result;
        }}

        public override void Write(
            Utf8JsonWriter writer,
            {Declaration.ToDisplayString()} value,
            JsonSerializerOptions options)
        {{
            writer.WriteNullValue();
        }}
    }}
}}";
        }
    }

    internal record DiscriminatedUnionMemberSerializedType(
        INamedTypeSymbol Declaration,
        TypedConstant DiscriminatorValue)
        : SerializedType(Declaration)
    {
        public DiscriminatedUnionSerializedType? DiscriminatedUnionDeclaration
            { get; init; }

        protected override IEnumerable<IPropertySymbol> SymbolsToSerialize
            => base.SymbolsToSerialize
                .Where(x => !SymbolEqualityComparer.Default.Equals(x,
                    DiscriminatedUnionDeclaration?.Discriminator));
    }
}
