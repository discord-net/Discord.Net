using Discord.API;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Serialization.Converters
{
    internal static class Converter
    {
        private static readonly MethodInfo _makeListConverterFunc
            = typeof(Converter).GetTypeInfo().GetDeclaredMethod(nameof(MakeListConverterInternal));
        private static readonly MethodInfo _makeOptionalConverterFunc 
            = typeof(Converter).GetTypeInfo().GetDeclaredMethod(nameof(MakeOptionalConverterInternal));
        private static readonly MethodInfo _makeNullableConverterFunc 
            = typeof(Converter).GetTypeInfo().GetDeclaredMethod(nameof(MakeNullableConverterInternal));
        private static readonly MethodInfo _makeEntityOrIdConverterFunc 
            = typeof(Converter).GetTypeInfo().GetDeclaredMethod(nameof(MakeEntityOrIdConverterInternal));

        public static IPropertyConverter<TProp> For<TProp>()
            => (IPropertyConverter<TProp>)ForInternal<TProp>();
        private static object ForInternal<TProp>()
        {
            var typeInfo = typeof(TProp).GetTypeInfo();

            //Generics
            if (typeof(TProp).IsConstructedGenericType)
            {
                Type genericType = typeof(TProp).GetGenericTypeDefinition();
                if (genericType == typeof(List<>))
                    return MakeListConverter<TProp>(typeof(TProp).GenericTypeArguments[0]);
                else if (genericType == typeof(Optional<>))
                    return MakeOptionalConverter<TProp>(typeof(TProp).GenericTypeArguments[0]);
                else if (genericType == typeof(Nullable<>))
                    return MakeNullableConverter<TProp>(typeof(TProp).GenericTypeArguments[0]);
                else if (genericType == typeof(EntityOrId<>))
                    return MakeEntityOrIdConverter<TProp>(typeof(TProp).GenericTypeArguments[0]);
            }

            //Enums
            if (typeInfo.IsEnum) return new EnumPropertyConverter<TProp>();

            //Primitives
            if (typeof(TProp) == typeof(bool)) return new BooleanPropertyConverter();

            if (typeof(TProp) == typeof(sbyte)) return new Int8PropertyConverter();
            if (typeof(TProp) == typeof(short)) return new Int16PropertyConverter();
            if (typeof(TProp) == typeof(int)) return new Int32PropertyConverter();
            if (typeof(TProp) == typeof(long))
            {
                if (typeInfo.GetCustomAttribute<Int53Attribute>() != null)
                    return new Int53PropertyConverter();
                else
                    return new Int64PropertyConverter();
            }

            if (typeof(TProp) == typeof(byte)) return new UInt8PropertyConverter();
            if (typeof(TProp) == typeof(ushort)) return new UInt16PropertyConverter();
            if (typeof(TProp) == typeof(uint)) return new UInt32PropertyConverter();
            if (typeof(TProp) == typeof(ulong))
            {
                if (typeInfo.GetCustomAttribute<Int53Attribute>() != null)
                    return new UInt53PropertyConverter();
                else
                    return new UInt64PropertyConverter();
            }

            if (typeof(TProp) == typeof(float)) return new SinglePropertyConverter();
            if (typeof(TProp) == typeof(double)) return new DoublePropertyConverter();
            if (typeof(TProp) == typeof(decimal)) return new DecimalPropertyConverter();

            if (typeof(TProp) == typeof(char)) return new CharPropertyConverter();
            if (typeof(TProp) == typeof(string)) return new StringPropertyConverter();

            //Structs
            if (typeof(TProp) == typeof(DateTime)) return new DateTimePropertyConverter();
            if (typeof(TProp) == typeof(DateTimeOffset)) return new DateTimeOffsetPropertyConverter();
            if (typeof(TProp) == typeof(Image)) return new ImagePropertyConverter();

            throw new InvalidOperationException($"Unsupported model type: {typeof(TProp).Name}");
        }

        private static IPropertyConverter<TProp> MakeListConverter<TProp>(Type innerType)
            => _makeListConverterFunc.MakeGenericMethod(innerType).Invoke(null, null) as IPropertyConverter<TProp>;
        private static IPropertyConverter<List<TInnerProp>> MakeListConverterInternal<TInnerProp>()
            => new ListPropertyConverter<TInnerProp>(For<TInnerProp>());

        private static IPropertyConverter<TProp> MakeOptionalConverter<TProp>(Type innerType)
            => _makeOptionalConverterFunc.MakeGenericMethod(innerType).Invoke(null, null) as IPropertyConverter<TProp>;
        private static IPropertyConverter<Optional<TInnerProp>> MakeOptionalConverterInternal<TInnerProp>()
            => new OptionalPropertyConverter<TInnerProp>(For<TInnerProp>());

        private static IPropertyConverter<TProp> MakeNullableConverter<TProp>(Type innerType)
            => _makeNullableConverterFunc.MakeGenericMethod(innerType).Invoke(null, null) as IPropertyConverter<TProp>;
        private static IPropertyConverter<TInnerProp?> MakeNullableConverterInternal<TInnerProp>()
            where TInnerProp : struct
            => new NullablePropertyConverter<TInnerProp>(For<TInnerProp>());

        private static IPropertyConverter<TProp> MakeEntityOrIdConverter<TProp>(Type innerType)
            => _makeEntityOrIdConverterFunc.MakeGenericMethod(innerType).Invoke(null, null) as IPropertyConverter<TProp>;
        private static IPropertyConverter<EntityOrId<TInnerProp>> MakeEntityOrIdConverterInternal<TInnerProp>()
            => new EntityOrIdPropertyConverter<TInnerProp>(For<TInnerProp>());
    }
}
