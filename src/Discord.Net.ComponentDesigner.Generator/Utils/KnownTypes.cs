using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Discord.CX;

public class KnownTypes
{
    public Compilation Compilation { get; }

    public KnownTypes(Compilation compilation)
    {
        Compilation = compilation;
    }

    public INamedTypeSymbol? ICXElementType
        => GetOrResolveType("InlineComponent.ICXElement", ref _ICXElementType);

    private Optional<INamedTypeSymbol?> _ICXElementType;

    public INamedTypeSymbol? ColorType
        => GetOrResolveType("Discord.Color", ref _ColorType);

    private Optional<INamedTypeSymbol?> _ColorType;

    public INamedTypeSymbol? EmojiType
        => GetOrResolveType("Discord.Emoji", ref _EmojiType);

    private Optional<INamedTypeSymbol?> _EmojiType;

    public INamedTypeSymbol? IEmoteType
        => GetOrResolveType("Discord.IEmote", ref _IEmoteType);

    private Optional<INamedTypeSymbol?> _IEmoteType;

    public INamedTypeSymbol? EmoteType
        => GetOrResolveType("Discord.Emote", ref _EmoteType);

    private Optional<INamedTypeSymbol?> _EmoteType;

    public INamedTypeSymbol? ButtonStyleEnumType
        => GetOrResolveType("Discord.ButtonStyle", ref _ButtonStyleEnumType);

    private Optional<INamedTypeSymbol?> _ButtonStyleEnumType;

    public INamedTypeSymbol? ComponentTypeEnumType
        => GetOrResolveType("Discord.ComponentType", ref _ComponentTypeEnumType);

    private Optional<INamedTypeSymbol?> _ComponentTypeEnumType;

    public INamedTypeSymbol? IMessageComponentBuilderType
        => GetOrResolveType("Discord.IMessageComponentBuilder", ref _IMessageComponentBuilderType);

    private Optional<INamedTypeSymbol?> _IMessageComponentBuilderType;

    public INamedTypeSymbol? ActionRowBuilderType
        => GetOrResolveType("Discord.ActionRowBuilder", ref _ActionRowBuilderType);

    private Optional<INamedTypeSymbol?> _ActionRowBuilderType;

    public INamedTypeSymbol? ButtonBuilderType
        => GetOrResolveType("Discord.ButtonBuilder", ref _ButtonBuilderType);

    private Optional<INamedTypeSymbol?> _ButtonBuilderType;

    public INamedTypeSymbol? ComponentBuilderType
        => GetOrResolveType("Discord.ComponentBuilder", ref _ComponentBuilderType);

    private Optional<INamedTypeSymbol?> _ComponentBuilderType;

    public INamedTypeSymbol? ComponentBuilderV2Type
        => GetOrResolveType("Discord.ComponentBuilderV2", ref _ComponentBuilderV2Type);

    private Optional<INamedTypeSymbol?> _ComponentBuilderV2Type;

    public INamedTypeSymbol? ContainerBuilderType
        => GetOrResolveType("Discord.ContainerBuilder", ref _ContainerBuilderType);

    private Optional<INamedTypeSymbol?> _ContainerBuilderType;

    public INamedTypeSymbol? FileComponentBuilderType
        => GetOrResolveType("Discord.FileComponentBuilder", ref _FileComponentBuilderType);

    private Optional<INamedTypeSymbol?> _FileComponentBuilderType;

    public INamedTypeSymbol? MediaGalleryBuilderType
        => GetOrResolveType("Discord.MediaGalleryBuilder", ref _MediaGalleryBuilderType);

    private Optional<INamedTypeSymbol?> _MediaGalleryBuilderType;

    public INamedTypeSymbol? MediaGalleryItemPropertiesType
        => GetOrResolveType("Discord.MediaGalleryItemProperties", ref _MediaGalleryItemPropertiesType);

    private Optional<INamedTypeSymbol?> _MediaGalleryItemPropertiesType;

    public INamedTypeSymbol? SectionBuilderType
        => GetOrResolveType("Discord.SectionBuilder", ref _SectionBuilderType);

    private Optional<INamedTypeSymbol?> _SectionBuilderType;

    public INamedTypeSymbol? SelectMenuBuilderType
        => GetOrResolveType("Discord.SelectMenuBuilder", ref _SelectMenuBuilderType);

    private Optional<INamedTypeSymbol?> _SelectMenuBuilderType;

    public INamedTypeSymbol? SelectMenuOptionBuilderType
        => GetOrResolveType("Discord.SelectMenuOptionBuilder", ref _SelectMenuOptionBuilderType);

    private Optional<INamedTypeSymbol?> _SelectMenuOptionBuilderType;

    public INamedTypeSymbol? SelectMenuDefaultValueType
        => GetOrResolveType("Discord.SelectMenuDefaultValue", ref _SelectMenuDefaultValueType);

    private Optional<INamedTypeSymbol?> _SelectMenuDefaultValueType;

    public INamedTypeSymbol? SelectDefaultValueTypeEnumType
        => GetOrResolveType("Discord.SelectDefaultValueType", ref _SelectDefaultValueTypeEnumType);

    private Optional<INamedTypeSymbol?> _SelectDefaultValueTypeEnumType;

    public INamedTypeSymbol? SeparatorBuilderType
        => GetOrResolveType("Discord.SeparatorBuilder", ref _SeparatorBuilderType);

    private Optional<INamedTypeSymbol?> _SeparatorBuilderType;

    public INamedTypeSymbol? SeparatorSpacingSizeType
        => GetOrResolveType("Discord.SeparatorSpacingSize", ref _SeparatorSpacingSizeType);

    private Optional<INamedTypeSymbol?> _SeparatorSpacingSizeType;

    public INamedTypeSymbol? TextDisplayBuilderType
        => GetOrResolveType("Discord.TextDisplayBuilder", ref _TextDisplayBuilderType);

    private Optional<INamedTypeSymbol?> _TextDisplayBuilderType;

    public INamedTypeSymbol? TextInputBuilderType
        => GetOrResolveType("Discord.TextInputBuilder", ref _TextInputBuilderType);

    private Optional<INamedTypeSymbol?> _TextInputBuilderType;

    public INamedTypeSymbol? ThumbnailBuilderType
        => GetOrResolveType("Discord.ThumbnailBuilder", ref _ThumbnailBuilderType);

    private Optional<INamedTypeSymbol?> _ThumbnailBuilderType;

    public INamedTypeSymbol? UnfurledMediaItemPropertiesType
        => GetOrResolveType("Discord.UnfurledMediaItemProperties", ref _UnfurledMediaItemPropertiesType);

    private Optional<INamedTypeSymbol?> _UnfurledMediaItemPropertiesType;


    public HashSet<ITypeSymbol>? BuiltInSupportTypes { get; set; }

    public INamedTypeSymbol? IListOfTType => GetOrResolveType(typeof(IList<>), ref _IListOfTType);
    private Optional<INamedTypeSymbol?> _IListOfTType;

    public INamedTypeSymbol? ICollectionOfTType => GetOrResolveType(typeof(ICollection<>), ref _ICollectionOfTType);
    private Optional<INamedTypeSymbol?> _ICollectionOfTType;

    public INamedTypeSymbol? IEnumerableType => GetOrResolveType(typeof(IEnumerable), ref _IEnumerableType);
    private Optional<INamedTypeSymbol?> _IEnumerableType;

    public INamedTypeSymbol? IEnumerableOfTType => GetOrResolveType(typeof(IEnumerable<>), ref _IEnumerableOfTType);
    private Optional<INamedTypeSymbol?> _IEnumerableOfTType;

    public INamedTypeSymbol? ListOfTType => GetOrResolveType(typeof(List<>), ref _ListOfTType);
    private Optional<INamedTypeSymbol?> _ListOfTType;

    public INamedTypeSymbol? DictionaryOfTKeyTValueType =>
        GetOrResolveType(typeof(Dictionary<,>), ref _DictionaryOfTKeyTValueType);

    private Optional<INamedTypeSymbol?> _DictionaryOfTKeyTValueType;

    public INamedTypeSymbol? IAsyncEnumerableOfTType =>
        GetOrResolveType("System.Collections.Generic.IAsyncEnumerable`1", ref _AsyncEnumerableOfTType);

    private Optional<INamedTypeSymbol?> _AsyncEnumerableOfTType;

    public INamedTypeSymbol? IDictionaryOfTKeyTValueType =>
        GetOrResolveType(typeof(IDictionary<,>), ref _IDictionaryOfTKeyTValueType);

    private Optional<INamedTypeSymbol?> _IDictionaryOfTKeyTValueType;

    public INamedTypeSymbol? IReadonlyDictionaryOfTKeyTValueType => GetOrResolveType(typeof(IReadOnlyDictionary<,>),
        ref _IReadonlyDictionaryOfTKeyTValueType);

    private Optional<INamedTypeSymbol?> _IReadonlyDictionaryOfTKeyTValueType;

    public INamedTypeSymbol? ISetOfTType => GetOrResolveType(typeof(ISet<>), ref _ISetOfTType);
    private Optional<INamedTypeSymbol?> _ISetOfTType;

    public INamedTypeSymbol? StackOfTType => GetOrResolveType(typeof(Stack<>), ref _StackOfTType);
    private Optional<INamedTypeSymbol?> _StackOfTType;

    public INamedTypeSymbol? QueueOfTType => GetOrResolveType(typeof(Queue<>), ref _QueueOfTType);
    private Optional<INamedTypeSymbol?> _QueueOfTType;

    public INamedTypeSymbol? ConcurrentStackType =>
        GetOrResolveType(typeof(ConcurrentStack<>), ref _ConcurrentStackType);

    private Optional<INamedTypeSymbol?> _ConcurrentStackType;

    public INamedTypeSymbol? ConcurrentQueueType =>
        GetOrResolveType(typeof(ConcurrentQueue<>), ref _ConcurrentQueueType);

    private Optional<INamedTypeSymbol?> _ConcurrentQueueType;

    public INamedTypeSymbol? IDictionaryType => GetOrResolveType(typeof(IDictionary), ref _IDictionaryType);
    private Optional<INamedTypeSymbol?> _IDictionaryType;

    public INamedTypeSymbol? IListType => GetOrResolveType(typeof(IList), ref _IListType);
    private Optional<INamedTypeSymbol?> _IListType;

    public INamedTypeSymbol? StackType => GetOrResolveType(typeof(Stack), ref _StackType);
    private Optional<INamedTypeSymbol?> _StackType;

    public INamedTypeSymbol? QueueType => GetOrResolveType(typeof(Queue), ref _QueueType);
    private Optional<INamedTypeSymbol?> _QueueType;

    public INamedTypeSymbol? KeyValuePair => GetOrResolveType(typeof(KeyValuePair<,>), ref _KeyValuePair);
    private Optional<INamedTypeSymbol?> _KeyValuePair;

    public INamedTypeSymbol? ImmutableArrayType => GetOrResolveType(typeof(ImmutableArray<>), ref _ImmutableArrayType);
    private Optional<INamedTypeSymbol?> _ImmutableArrayType;

    public INamedTypeSymbol? ImmutableListType => GetOrResolveType(typeof(ImmutableList<>), ref _ImmutableListType);
    private Optional<INamedTypeSymbol?> _ImmutableListType;

    public INamedTypeSymbol? IImmutableListType => GetOrResolveType(typeof(IImmutableList<>), ref _IImmutableListType);
    private Optional<INamedTypeSymbol?> _IImmutableListType;

    public INamedTypeSymbol? ImmutableStackType => GetOrResolveType(typeof(ImmutableStack<>), ref _ImmutableStackType);
    private Optional<INamedTypeSymbol?> _ImmutableStackType;

    public INamedTypeSymbol? IImmutableStackType =>
        GetOrResolveType(typeof(IImmutableStack<>), ref _IImmutableStackType);

    private Optional<INamedTypeSymbol?> _IImmutableStackType;

    public INamedTypeSymbol? ImmutableQueueType => GetOrResolveType(typeof(ImmutableQueue<>), ref _ImmutableQueueType);
    private Optional<INamedTypeSymbol?> _ImmutableQueueType;

    public INamedTypeSymbol? IImmutableQueueType =>
        GetOrResolveType(typeof(IImmutableQueue<>), ref _IImmutableQueueType);

    private Optional<INamedTypeSymbol?> _IImmutableQueueType;

    public INamedTypeSymbol? ImmutableSortedType =>
        GetOrResolveType(typeof(ImmutableSortedSet<>), ref _ImmutableSortedType);

    private Optional<INamedTypeSymbol?> _ImmutableSortedType;

    public INamedTypeSymbol? ImmutableHashSetType =>
        GetOrResolveType(typeof(ImmutableHashSet<>), ref _ImmutableHashSetType);

    private Optional<INamedTypeSymbol?> _ImmutableHashSetType;

    public INamedTypeSymbol? IImmutableSetType => GetOrResolveType(typeof(IImmutableSet<>), ref _IImmutableSetType);
    private Optional<INamedTypeSymbol?> _IImmutableSetType;

    public INamedTypeSymbol? ImmutableDictionaryType =>
        GetOrResolveType(typeof(ImmutableDictionary<,>), ref _ImmutableDictionaryType);

    private Optional<INamedTypeSymbol?> _ImmutableDictionaryType;

    public INamedTypeSymbol? ImmutableSortedDictionaryType =>
        GetOrResolveType(typeof(ImmutableSortedDictionary<,>), ref _ImmutableSortedDictionaryType);

    private Optional<INamedTypeSymbol?> _ImmutableSortedDictionaryType;

    public INamedTypeSymbol? IImmutableDictionaryType =>
        GetOrResolveType(typeof(IImmutableDictionary<,>), ref _IImmutableDictionaryType);

    private Optional<INamedTypeSymbol?> _IImmutableDictionaryType;

    public INamedTypeSymbol? KeyedCollectionType =>
        GetOrResolveType(typeof(KeyedCollection<,>), ref _KeyedCollectionType);

    private Optional<INamedTypeSymbol?> _KeyedCollectionType;

    public INamedTypeSymbol ObjectType => _ObjectType ??= Compilation.GetSpecialType(SpecialType.System_Object);
    private INamedTypeSymbol? _ObjectType;

    public INamedTypeSymbol StringType => _StringType ??= Compilation.GetSpecialType(SpecialType.System_String);
    private INamedTypeSymbol? _StringType;

    public INamedTypeSymbol? DateTimeOffsetType => GetOrResolveType(typeof(DateTimeOffset), ref _DateTimeOffsetType);
    private Optional<INamedTypeSymbol?> _DateTimeOffsetType;

    public INamedTypeSymbol? TimeSpanType => GetOrResolveType(typeof(TimeSpan), ref _TimeSpanType);
    private Optional<INamedTypeSymbol?> _TimeSpanType;

    public INamedTypeSymbol? DateOnlyType => GetOrResolveType("System.DateOnly", ref _DateOnlyType);
    private Optional<INamedTypeSymbol?> _DateOnlyType;

    public INamedTypeSymbol? TimeOnlyType => GetOrResolveType("System.TimeOnly", ref _TimeOnlyType);
    private Optional<INamedTypeSymbol?> _TimeOnlyType;

    public INamedTypeSymbol? Int128Type => GetOrResolveType("System.Int128", ref _Int128Type);
    private Optional<INamedTypeSymbol?> _Int128Type;

    public INamedTypeSymbol? UInt128Type => GetOrResolveType("System.UInt128", ref _UInt128Type);
    private Optional<INamedTypeSymbol?> _UInt128Type;

    public INamedTypeSymbol? HalfType => GetOrResolveType("System.Half", ref _HalfType);
    private Optional<INamedTypeSymbol?> _HalfType;

    public IArrayTypeSymbol? ByteArrayType => _ByteArrayType.HasValue
        ? _ByteArrayType.Value
        : (_ByteArrayType =
            new(Compilation.CreateArrayTypeSymbol(Compilation.GetSpecialType(SpecialType.System_Byte), rank: 1))).Value;

    private Optional<IArrayTypeSymbol?> _ByteArrayType;

    public INamedTypeSymbol? MemoryByteType => _MemoryByteType.HasValue
        ? _MemoryByteType.Value
        : (_MemoryByteType = new(MemoryType?.Construct(Compilation.GetSpecialType(SpecialType.System_Byte)))).Value;

    private Optional<INamedTypeSymbol?> _MemoryByteType;

    public INamedTypeSymbol? ReadOnlyMemoryByteType => _ReadOnlyMemoryByteType.HasValue
        ? _ReadOnlyMemoryByteType.Value
        : (_ReadOnlyMemoryByteType =
            new(ReadOnlyMemoryType?.Construct(Compilation.GetSpecialType(SpecialType.System_Byte)))).Value;

    private Optional<INamedTypeSymbol?> _ReadOnlyMemoryByteType;

    public INamedTypeSymbol? GuidType => GetOrResolveType(typeof(Guid), ref _GuidType);
    private Optional<INamedTypeSymbol?> _GuidType;

    public INamedTypeSymbol? UriType => GetOrResolveType(typeof(Uri), ref _UriType);
    private Optional<INamedTypeSymbol?> _UriType;

    public INamedTypeSymbol? VersionType => GetOrResolveType(typeof(Version), ref _VersionType);
    private Optional<INamedTypeSymbol?> _VersionType;

    // Unsupported types
    public INamedTypeSymbol? DelegateType => _DelegateType ??= Compilation.GetSpecialType(SpecialType.System_Delegate);
    private INamedTypeSymbol? _DelegateType;

    public INamedTypeSymbol? MemberInfoType => GetOrResolveType(typeof(MemberInfo), ref _MemberInfoType);
    private Optional<INamedTypeSymbol?> _MemberInfoType;

    public INamedTypeSymbol? IntPtrType => GetOrResolveType(typeof(IntPtr), ref _IntPtrType);
    private Optional<INamedTypeSymbol?> _IntPtrType;

    public INamedTypeSymbol? UIntPtrType => GetOrResolveType(typeof(UIntPtr), ref _UIntPtrType);
    private Optional<INamedTypeSymbol?> _UIntPtrType;

    public INamedTypeSymbol? MemoryType => GetOrResolveType(typeof(Memory<>), ref _MemoryType);
    private Optional<INamedTypeSymbol?> _MemoryType;

    public INamedTypeSymbol? ReadOnlyMemoryType => GetOrResolveType(typeof(ReadOnlyMemory<>), ref _ReadOnlyMemoryType);
    private Optional<INamedTypeSymbol?> _ReadOnlyMemoryType;

    public bool IsImmutableEnumerableType(ITypeSymbol type, out string? factoryTypeFullName)
    {
        if (type is not INamedTypeSymbol { IsGenericType: true, ConstructedFrom: INamedTypeSymbol genericTypeDef })
        {
            factoryTypeFullName = null;
            return false;
        }

        SymbolEqualityComparer cmp = SymbolEqualityComparer.Default;
        if (cmp.Equals(genericTypeDef, ImmutableArrayType))
        {
            factoryTypeFullName = typeof(ImmutableArray).FullName;
            return true;
        }

        if (cmp.Equals(genericTypeDef, ImmutableListType) ||
            cmp.Equals(genericTypeDef, IImmutableListType))
        {
            factoryTypeFullName = typeof(ImmutableList).FullName;
            return true;
        }

        if (cmp.Equals(genericTypeDef, ImmutableStackType) ||
            cmp.Equals(genericTypeDef, IImmutableStackType))
        {
            factoryTypeFullName = typeof(ImmutableStack).FullName;
            return true;
        }

        if (cmp.Equals(genericTypeDef, ImmutableQueueType) ||
            cmp.Equals(genericTypeDef, IImmutableQueueType))
        {
            factoryTypeFullName = typeof(ImmutableQueue).FullName;
            return true;
        }

        if (cmp.Equals(genericTypeDef, ImmutableHashSetType) ||
            cmp.Equals(genericTypeDef, IImmutableSetType))
        {
            factoryTypeFullName = typeof(ImmutableHashSet).FullName;
            return true;
        }

        if (cmp.Equals(genericTypeDef, ImmutableSortedType))
        {
            factoryTypeFullName = typeof(ImmutableSortedSet).FullName;
            return true;
        }

        factoryTypeFullName = null;
        return false;
    }

    public bool IsImmutableDictionaryType(ITypeSymbol type, out string? factoryTypeFullName)
    {
        if (type is not INamedTypeSymbol {IsGenericType: true, ConstructedFrom: { } genericTypeDef})
        {
            factoryTypeFullName = null;
            return false;
        }

        SymbolEqualityComparer cmp = SymbolEqualityComparer.Default;

        if (cmp.Equals(genericTypeDef, ImmutableDictionaryType) ||
            cmp.Equals(genericTypeDef, IImmutableDictionaryType))
        {
            factoryTypeFullName = typeof(ImmutableDictionary).FullName;
            return true;
        }

        if (cmp.Equals(genericTypeDef, ImmutableSortedDictionaryType))
        {
            factoryTypeFullName = typeof(ImmutableSortedDictionary).FullName;
            return true;
        }

        factoryTypeFullName = null;
        return false;
    }


    private INamedTypeSymbol? GetOrResolveType(Type type, ref Optional<INamedTypeSymbol?> field)
        => GetOrResolveType(type.FullName!, ref field);

    private INamedTypeSymbol? GetOrResolveType(string fullyQualifiedName, ref Optional<INamedTypeSymbol?> field)
    {
        if (field.HasValue)
        {
            return field.Value;
        }

        var type = GetBestType();
        field = new(type);
        return type;

        INamedTypeSymbol? GetBestType()
        {
            var type = Compilation.GetTypeByMetadataName(fullyQualifiedName) ??
                       Compilation.Assembly.GetTypeByMetadataName(fullyQualifiedName);

            if (type is null)
            {
                foreach (var module in Compilation.Assembly.Modules)
                {
                    foreach (var referencedAssembly in module.ReferencedAssemblySymbols)
                    {
                        var currentType = referencedAssembly.GetTypeByMetadataName(fullyQualifiedName);
                        if (currentType is null)
                            continue;

                        var visibility = Accessibility.Public;

                        ISymbol symbol = currentType;
                        while (symbol is not null && symbol.Kind is not SymbolKind.Namespace)
                        {
                            switch (currentType.DeclaredAccessibility)
                            {
                                case Accessibility.Private or Accessibility.NotApplicable:
                                    visibility = Accessibility.Private;
                                    break;
                                case Accessibility.Internal or Accessibility.ProtectedAndInternal:
                                    visibility = Accessibility.Internal;
                                    break;
                            }

                            symbol = symbol.ContainingSymbol;
                        }

                        switch (visibility)
                        {
                            case Accessibility.Public:
                            case Accessibility.Internal when referencedAssembly.GivesAccessTo(Compilation.Assembly):
                                break;

                            default:
                                continue;
                        }

                        if (type is object)
                        {
                            return null;
                        }

                        type = currentType;
                    }
                }
            }

            return type;
        }
    }
}


public static class KnownTypesExtensions
{
    private static readonly ConditionalWeakTable<Compilation, KnownTypes> _table = new();

    public static KnownTypes GetKnownTypes(this Compilation compilation)
    {
        if (_table.TryGetValue(compilation, out var knownTypes))
            return knownTypes;

        knownTypes = new(compilation);
        _table.Add(compilation, knownTypes);
        return knownTypes;
    }
}
