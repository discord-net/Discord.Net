using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json;

partial class DiscordJsonContext
{
    private bool TryGetBuiltIn(Type type, [MaybeNullWhen(false)] out JsonTypeInfo builtIn)
    {
        if (type == typeof(byte)) return (builtIn = Byte) is not null;
        if (type == typeof(sbyte)) return (builtIn = SByte) is not null;
        if (type == typeof(short)) return (builtIn = Int16) is not null;
        if (type == typeof(ushort)) return (builtIn = UInt16) is not null;
        if (type == typeof(int)) return (builtIn = Int32) is not null;
        if (type == typeof(uint)) return (builtIn = UInt32) is not null;
        if (type == typeof(long)) return (builtIn = Int64) is not null;
        if (type == typeof(ulong)) return (builtIn = UInt64) is not null;
        if (type == typeof(double)) return (builtIn = Double) is not null;
        if (type == typeof(float)) return (builtIn = Single) is not null;
        if (type == typeof(decimal)) return (builtIn = Decimal) is not null;
        if (type == typeof(bool)) return (builtIn = Boolean) is not null;
        if (type == typeof(char)) return (builtIn = Char) is not null;
        if (type == typeof(string)) return (builtIn = String) is not null;
        
        builtIn = null;
        return false;
    }

    [field: MaybeNull]
    public JsonTypeInfo<byte> Byte
        => field ??= JsonMetadataServices.CreateValueInfo<byte>(Options, JsonMetadataServices.ByteConverter);
    [field: MaybeNull]
    public JsonTypeInfo<sbyte> SByte
        => field ??= JsonMetadataServices.CreateValueInfo<sbyte>(Options, JsonMetadataServices.SByteConverter);
    [field: MaybeNull]
    public JsonTypeInfo<short> Int16
        => field ??= JsonMetadataServices.CreateValueInfo<short>(Options, JsonMetadataServices.Int16Converter);
    [field: MaybeNull]
    public JsonTypeInfo<ushort> UInt16
        => field ??= JsonMetadataServices.CreateValueInfo<ushort>(Options, JsonMetadataServices.UInt16Converter);
    [field: MaybeNull]
    public JsonTypeInfo<int> Int32
        => field ??= JsonMetadataServices.CreateValueInfo<int>(Options, JsonMetadataServices.Int32Converter);
    [field: MaybeNull]
    public JsonTypeInfo<uint> UInt32
        => field ??= JsonMetadataServices.CreateValueInfo<uint>(Options, JsonMetadataServices.UInt32Converter);
    [field: MaybeNull]
    public JsonTypeInfo<long> Int64
        => field ??= JsonMetadataServices.CreateValueInfo<long>(Options, JsonMetadataServices.Int64Converter);
    [field: MaybeNull]
    public JsonTypeInfo<ulong> UInt64
        => field ??= JsonMetadataServices.CreateValueInfo<ulong>(Options, JsonMetadataServices.UInt64Converter);
    [field: MaybeNull]
    public JsonTypeInfo<double> Double
        => field ??= JsonMetadataServices.CreateValueInfo<double>(Options, JsonMetadataServices.DoubleConverter);
    [field: MaybeNull]
    public JsonTypeInfo<float> Single
        => field ??= JsonMetadataServices.CreateValueInfo<float>(Options, JsonMetadataServices.SingleConverter);
    [field: MaybeNull]
    public JsonTypeInfo<decimal> Decimal
        => field ??= JsonMetadataServices.CreateValueInfo<decimal>(Options, JsonMetadataServices.DecimalConverter);
    [field: MaybeNull]
    public JsonTypeInfo<bool> Boolean
        => field ??= JsonMetadataServices.CreateValueInfo<bool>(Options, JsonMetadataServices.BooleanConverter);
    [field: MaybeNull]
    public JsonTypeInfo<char> Char
        => field ??= JsonMetadataServices.CreateValueInfo<char>(Options, JsonMetadataServices.CharConverter);
    [field: MaybeNull]
    public JsonTypeInfo<string> String
        => field ??= JsonMetadataServices.CreateValueInfo<string>(Options, JsonMetadataServices.StringConverter);
}