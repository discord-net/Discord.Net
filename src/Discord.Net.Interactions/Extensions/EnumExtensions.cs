using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions
{
    internal static class EnumExtensions
    {
        public static IEnumerable<T> GetFlags<T>(this T flags) where T : struct, Enum
        {
            if (!typeof(T).IsEnum || typeof(T).IsDefined(typeof(FlagsAttribute), false))
                throw new ArgumentException($"{typeof(T).FullName} isn't a flags enum.", nameof(T));

            return Enum.GetValues<T>().Where(x => flags.HasFlag(x));
        }

        public static TypeReaderTarget ToTypeReaderTarget(this ComponentType componentType) =>
            (componentType) switch
            {
                ComponentType.SelectMenu => TypeReaderTarget.SelectMenu,
                _ => throw new InvalidOperationException($"{componentType} isn't supported by {nameof(CompTypeConverter)}s.");
            };
    }
}
