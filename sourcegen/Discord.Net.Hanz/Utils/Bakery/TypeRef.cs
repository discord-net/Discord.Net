using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public sealed class TypeRef(ITypeSymbol type) : IEquatable<TypeRef>
{
    public static readonly SymbolDisplayFormat DeclarationFormat = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        kindOptions: SymbolDisplayKindOptions.None,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );
    
    public string Name { get; } = type.Name;
    public string Namespace { get; } = type.ContainingNamespace.ToString();

    public string DisplayString { get; } = type.ToDisplayString();
    public string MetadataName { get; } = type.ToFullMetadataName();
    public string ReferenceName { get; } = type.ToDisplayString(DeclarationFormat);
    
    public string FullyQualifiedName { get; } = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    public Accessibility Accessibility { get; } = type.DeclaredAccessibility;

    public bool IsValueType { get; } = type.IsValueType;
    public TypeKind TypeKind { get; } = type.TypeKind;
    public SpecialType SpecialType { get; } = type.OriginalDefinition.SpecialType;

    public bool CanBeNull => !IsValueType || SpecialType is SpecialType.System_Nullable_T;

    public override string ToString() => FullyQualifiedName;

    public bool Equals(TypeRef? other) => other != null && FullyQualifiedName == other.FullyQualifiedName;
    public override bool Equals(object? obj) => Equals(obj as TypeRef);
    public override int GetHashCode() => HashCode.Of(FullyQualifiedName).And(Accessibility);
}