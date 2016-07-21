using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class EnumTypeReader : TypeReader
    {
        private readonly Dictionary<string, object> stringValues;
        private readonly Dictionary<int, object> intValues;
        private readonly Type enumType;

        public override Task<TypeReaderResult> Read(IMessage context, string input)
        {
            int inputAsInt;
            object enumValue;

            if (int.TryParse(input, out inputAsInt))
            {
                if (intValues.TryGetValue(inputAsInt, out enumValue))
                    return Task.FromResult(TypeReaderResult.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.CastFailed, $"Value is not a {enumType.Name}"));
            }
            else
            {
                if (stringValues.TryGetValue(input.ToLower(), out enumValue))
                    return Task.FromResult(TypeReaderResult.FromSuccess(enumValue));
                else
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.CastFailed, $"Value is not a {enumType.Name}"));
            }
        }

        public EnumTypeReader(Type type)
        {
            enumType = type;

            var stringValuesBuilder = new Dictionary<string, object>();
            var intValuesBuilder = new Dictionary<int, object>();

            var values = Enum.GetValues(enumType);

            foreach (var v in values)
            {
                stringValuesBuilder.Add(v.ToString().ToLower(), v);
                intValuesBuilder.Add((int)v, v);
            }

            stringValues = stringValuesBuilder;
            intValues = intValuesBuilder;
        }
    }
}
