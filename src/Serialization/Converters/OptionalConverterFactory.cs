using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discord.Net.Serialization.Converters
{
    /// <summary>
    /// Defines a converter factory which can be used to create instances of
    /// <see cref="OptionalConverter{T}"/>.
    /// </summary>
    public sealed class OptionalConverterFactory : JsonConverterFactory
    {
        private static readonly Type OptionalType = typeof(Optional<>);
        private static readonly Type OptionalConverterType = typeof(OptionalConverter<>);

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == OptionalType;

        /// <inheritdoc/>
        public override JsonConverter? CreateConverter(Type typeToConvert,
            JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert.IsGenericType);

            var underlyingType = typeToConvert.GenericTypeArguments[0];

            return (JsonConverter)Activator.CreateInstance(
                OptionalConverterType.MakeGenericType(underlyingType),
                args: new[] { options })!;
        }
    }
}
