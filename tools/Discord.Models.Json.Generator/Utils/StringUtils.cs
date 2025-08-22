using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Discord.Models.Json.Generator;

public static class StringUtils
{
    public static string WithNewlinePadding(this string str, int padding)
        => str.ReplaceLineEndings($"{Environment.NewLine}{"".PadRight(padding)}");

    public static string Prefix(this string str, int count, char c = ' ')
        => count is 0 ? str : $"{new string(c, count)}{str}";

    public static string ToSnakeCase(this string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.Length < 2)
        {
            return text.ToLowerInvariant();
        }

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (int i = 1; i < text.Length; ++i)
        {
            char c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public static string PropertyTypeToCodeString(this PropertyInfo property, bool typeReference = false)
        => property.PropertyType.ToCodeString(
            property.GetCustomAttribute<NullableAttribute>()?.NullableFlags,
            typeReference: typeReference
        );

    public static string ToCodeString(
        this Type type,
        byte[]? nullableContext = null,
        int depth = 0,
        bool typeReference = false
    )
    {
        var isNullable = nullableContext is not null && nullableContext.Length > depth && nullableContext[depth] is 2;

        var typeString = GetCodeString();

        if (isNullable && (!typeReference || type.IsValueType)) typeString += "?";

        return typeString;

        string GetCodeString()
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(char)) return "char";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(int)) return "int";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";

            if (type.IsArray && type.GetElementType() is { } element)
                return $"{element.ToCodeString(nullableContext, ++depth)}[]";

            var sb = new StringBuilder();

            if (type.Namespace is not "System" and not null)
                sb.Append(type.Namespace).Append('.');

            var name = type.Name;

            if (name.IndexOf('`') is not -1 and { } i)
                name = name[..i];

            sb.Append(name);

            if (type.GenericTypeArguments.Length > 0)
                sb.Append(
                    $"<{string.Join(",", type.GenericTypeArguments.Select(x => x.ToCodeString(nullableContext, ++depth)))}>");

            return sb.ToString();
        }
    }
}