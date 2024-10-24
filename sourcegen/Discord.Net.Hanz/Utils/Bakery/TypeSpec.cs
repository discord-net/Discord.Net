using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct TypeSpec(
    string Name,
    TypeKind Kind,
    Accessibility Accessibility = Accessibility.Public,
    ImmutableEquatableArray<string>? Bases = null,
    ImmutableEquatableArray<string>? Modifiers = null,
    ImmutableEquatableArray<TypeSpec>? Children = null,
    ImmutableEquatableArray<PropertySpec>? Properties = null,
    ImmutableEquatableArray<IndexerSpec>? Indexers = null,
    ImmutableEquatableArray<FieldSpec>? Fields = null,
    ImmutableEquatableArray<ConstructorSpec>? Constructors = null,
    ImmutableEquatableArray<MethodSpec>? Methods = null,
    ImmutableEquatableArray<GenericSpec>? Generics = null,
    ImmutableEquatableArray<GenericConstraintSpec>? GenericConstraints = null
)
{
    public bool IsDefault => this == default;

    public ImmutableEquatableArray<string> Bases { get; init; }
        = Bases ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<string> Modifiers { get; init; }
        = Modifiers ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<TypeSpec> Children { get; init; }
        = Children ?? ImmutableEquatableArray<TypeSpec>.Empty;

    public ImmutableEquatableArray<PropertySpec> Properties { get; init; }
        = Properties ?? ImmutableEquatableArray<PropertySpec>.Empty;

    public ImmutableEquatableArray<IndexerSpec> Indexers { get; init; }
        = Indexers ?? ImmutableEquatableArray<IndexerSpec>.Empty;

    public ImmutableEquatableArray<FieldSpec> Fields { get; init; }
        = Fields ?? ImmutableEquatableArray<FieldSpec>.Empty;

    public ImmutableEquatableArray<ConstructorSpec> Constructors { get; init; }
        = Constructors ?? ImmutableEquatableArray<ConstructorSpec>.Empty;

    public ImmutableEquatableArray<MethodSpec> Methods { get; init; }
        = Methods ?? ImmutableEquatableArray<MethodSpec>.Empty;

    public ImmutableEquatableArray<GenericSpec> Generics { get; init; }
        = Generics ?? ImmutableEquatableArray<GenericSpec>.Empty;

    public ImmutableEquatableArray<GenericConstraintSpec> GenericConstraints { get; init; }
        = GenericConstraints ?? ImmutableEquatableArray<GenericConstraintSpec>.Empty;

    public TypeSpec AddBases(params string[] bases)
        => this with {Bases = Bases.AddRange(bases.Where(x => !string.IsNullOrWhiteSpace(x)))};

    public TypeSpec AddBases(IEnumerable<string> bases)
        => this with {Bases = Bases.AddRange(bases.Where(x => !string.IsNullOrWhiteSpace(x)))};

    public TypeSpec AddModifiers(IEnumerable<string> modifiers)
        => this with {Modifiers = Modifiers.AddRange(modifiers.Where(x => !string.IsNullOrWhiteSpace(x)))};

    public TypeSpec AddModifiers(params string[] modifiers)
        => this with {Modifiers = Modifiers.AddRange(modifiers.Where(x => !string.IsNullOrWhiteSpace(x)))};

    public TypeSpec AddIndexers(
        params IndexerSpec[] indexer
    ) => this with {Indexers = Indexers.AddRange(indexer)};

    public TypeSpec AddIndexers(
        IEnumerable<IndexerSpec> indexers
    ) => this with {Indexers = Indexers.AddRange(indexers)};

    public TypeSpec AddMethods(
        params MethodSpec[] methods
    ) => this with {Methods = Methods.AddRange(methods)};

    public TypeSpec AddMethods(
        IEnumerable<MethodSpec> methods
    ) => this with {Methods = Methods.AddRange(methods)};

    public TypeSpec AddProperties(
        params PropertySpec[] properties
    ) => this with
    {
        Properties = Properties.AddRange(properties)
    };

    public TypeSpec AddProperties(
        IEnumerable<PropertySpec> properties
    ) => this with
    {
        Properties = Properties.AddRange(properties)
    };

    public TypeSpec AddNestedType(
        TypeSpec type
    ) => this with
    {
        Children = Children.Add(type)
    };

    public TypeSpec AddNestedTypes(
        IEnumerable<TypeSpec> types
    ) => this with
    {
        Children = Children.AddRange(types)
    };

    public TypeSpec AddInterfacePropertyOverload(
        string type,
        string interfaceName,
        string name,
        string expression
    ) => this with
    {
        Properties = Properties.Add(
            new PropertySpec(
                type,
                name,
                ExplicitInterfaceImplementation: interfaceName,
                Expression: expression
            )
        )
    };

    public TypeSpec AddInterfaceMethodOverload(
        string returnType,
        string interfaceName,
        string methodName,
        IEnumerable<ParameterSpec> parameters,
        string? expression = null,
        string? body = null
    ) => this with
    {
        Methods = Methods.Add(new MethodSpec(
            Name,
            returnType,
            ExplicitInterfaceImplementation: interfaceName,
            Parameters: new(parameters),
            Expression: expression,
            Body: body
        ))
    };

    public bool HasBrackets
        => Children.Count > 0 ||
           Properties.Count > 0;

    public static TypeSpec From(TypeRef typeref)
    {
        return new TypeSpec(
            Name: typeref.Name,
            Kind: typeref.TypeKind,
            Generics: new(typeref.Generics.Select(x => new GenericSpec(x))),
            GenericConstraints: new(typeref.GenericConstraints),
            Accessibility: typeref.Accessibility
        );
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(SyntaxFacts.GetText(Accessibility))
            .Append(' ');

        if (Modifiers.Count > 0)
        {
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');
        }

        builder
            .Append(Kind.ToString().ToLower())
            .Append(' ')
            .Append(Name);

        if (Generics.Count > 0)
        {
            builder
                .Append('<')
                .Append(string.Join(", ", Generics))
                .Append('>');
        }

        if (Bases.Count > 0)
        {
            builder
                .AppendLine(" : ")
                .Append(string.Join($",{Environment.NewLine}", Bases).PrefixSpaces(4).WithNewlinePadding(4));
        }

        if (GenericConstraints.Count > 0)
        {
            builder
                .AppendLine()
                .Append(string.Join(Environment.NewLine, GenericConstraints).PrefixSpaces(4).WithNewlinePadding(4));
        }

        if (HasBrackets)
        {
            builder
                .AppendLine()
                .AppendLine("{");

            var any = false;

            AddMembers(
                builder,
                Properties
                    .Where(x => x.ExplicitInterfaceImplementation is null)
                    .OrderByDescending(x => x.Accessibility),
                nameof(Properties),
                ref any,
                seperation: 2
            );

            AddMembers(
                builder,
                Indexers,
                nameof(Indexers),
                ref any,
                seperation: 2
            );

            AddMembers(
                builder,
                Fields,
                nameof(Fields),
                ref any
            );

            AddMembers(
                builder,
                Constructors,
                nameof(Constructors),
                ref any,
                seperation: 2
            );

            AddMembers(
                builder,
                Methods,
                nameof(Methods),
                ref any,
                seperation: 2
            );

            AddMembers(
                builder,
                Properties
                    .Where(x => x.ExplicitInterfaceImplementation is not null)
                    .OrderByDescending(x => x.Accessibility),
                nameof(Properties),
                ref any
            );

            AddMembers(
                builder,
                Children,
                nameof(Children),
                ref any,
                seperation: 2
            );

            builder.Append('}');
        }
        else builder.Append(';');

        return builder.ToString();
    }

    private void AddMembers<T>(
        StringBuilder builder,
        IEnumerable<T> members,
        string name,
        ref bool any,
        int seperation = 1,
        int padding = 4
    ) where T : struct, IEquatable<T>
    {
        var arr = members.Where(x => !x.Equals(default)).ToArray();

        try
        {
            if (arr.Length == 0) return;

            var formatted = string
                .Join(
                    string.Join(string.Empty, Enumerable.Range(0, seperation).Select(_ => Environment.NewLine)),
                    arr.Select(x => x.ToString().PrefixSpaces(padding).WithNewlinePadding(padding))
                );

            if (formatted == string.Empty) return;

            if (any)
            {
                for (var i = 0; i != seperation; i++)
                    builder.AppendLine();
            }

            any = true;

            builder.AppendLine(formatted);
        }
        catch (Exception ex)
        {
            var printed = new StringBuilder();
            PrintMembers(printed);

            throw new Exception($"Failed to add {name} members ({typeof(T)})\n{printed}", ex);
        }
    }
}