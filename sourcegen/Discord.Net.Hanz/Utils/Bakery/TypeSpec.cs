using System.Collections;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public record TypeSpec(
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
    ImmutableEquatableArray<GenericConstraintSpec>? GenericConstraints = null,
    ImmutableEquatableArray<ParameterSpec>? Parameters = null,
    bool Record = false
)
{
    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; } =
        Parameters ?? ImmutableEquatableArray<ParameterSpec>.Empty;

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

    public TypeSpec AddParameters(IEnumerable<ParameterSpec> parameters)
        => this with {Parameters = new([..Parameters, ..parameters])};
    
    public TypeSpec AddParameters(params ParameterSpec[] parameters)
        => this with {Parameters = new([..Parameters, ..parameters])};
    
    public TypeSpec AddBaseClass(string baseClass)
        => this with {Bases = new([baseClass, ..Bases])};
    
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

    public TypeSpec AddFields(
        params FieldSpec[] fields
    ) => this with {Fields = Fields.AddRange(fields)};
    
    public TypeSpec AddFields(
        IEnumerable<FieldSpec> fields
    ) => this with {Fields = Fields.AddRange(fields)};

    public TypeSpec AddNestedType(
        TypeSpec type
    ) => this with
    {
        Children = Children.Add(type)
    };

    public TypeSpec AddOrReplaceNestedType(TypeSpec spec)
    {
        var existing = Children.FirstOrDefault(x => x.Name == spec.Name);

        if (existing is null)
            return AddNestedType(spec);

        return this with {Children = Children.Remove(existing).Add(spec)};
    }

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
            methodName,
            returnType,
            ExplicitInterfaceImplementation: interfaceName,
            Parameters: new(parameters),
            Expression: expression,
            Body: body
        ))
    };

    public bool HasBrackets
        => Children.Count > 0 ||
           Properties.Count > 0 ||
           Methods.Count > 0 ||
           Fields.Count > 0 ||
           Indexers.Count > 0;

    public static TypeSpec From(TypeRef typeref)
    {
        return new TypeSpec(
            Name: typeref.Name,
            Kind: typeref.TypeKind,
            Generics: typeref.Generics.ToImmutableEquatableArray(),
            GenericConstraints: new(typeref.GenericConstraints),
            Accessibility: typeref.Accessibility,
            Record: typeref.IsRecord
        );
    }

    public string ToReferenceName()
    {
        if (Generics.Count == 0)
            return Name;
        
        var sb = new StringBuilder(Name);

        sb.Append('<');
        foreach (var generic in Generics)
            sb.Append(generic.Name).Append(", ");

        sb.Length -= 2;
        sb.Append('>');

        return sb.ToString();
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
                .Append(string.Join(" ", Modifiers.Distinct()))
                .Append(' ');
        }


        builder
            .Append(
                Record ? "record" : Kind.ToString().ToLower()
            )
            .Append(' ')
            .Append(Name);

        if (Generics.Count > 0)
        {
            builder
                .Append('<')
                .Append(string.Join(", ", Generics.Distinct()))
                .Append('>');
        }

        if (Parameters.Count > 0)
        {
            builder
                .Append('(')
                .Append(string.Join(", ", Parameters))
                .Append(')');
        }

        if (Bases.Count > 0)
        {
            builder
                .AppendLine(" : ")
                .Append(string.Join($",{Environment.NewLine}", Bases.Distinct()).Prefix(4).WithNewlinePadding(4));
        }

        if (GenericConstraints.Count > 0)
        {
            builder
                .AppendLine()
                .Append(string.Join(Environment.NewLine, GenericConstraints.Distinct()).Prefix(4)
                    .WithNewlinePadding(4));
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
                separation: 2
            );

            AddMembers(
                builder,
                Indexers,
                nameof(Indexers),
                ref any,
                separation: 2
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
                separation: 2
            );

            AddMembers(
                builder,
                Methods,
                nameof(Methods),
                ref any,
                separation: 2
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
                separation: 2
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
        int separation = 1,
        int padding = 4
    )
    {
        var arr = members.Where(x => x is not null).ToArray();

        try
        {
            if (arr.Length == 0) return;

            var formatted = string
                .Join(
                    string.Join(string.Empty, Enumerable.Range(0, separation).Select(_ => Environment.NewLine)),
                    arr.Select(x => x.ToString().Prefix(padding).WithNewlinePadding(padding))
                );

            if (formatted == string.Empty) return;

            if (any)
            {
                for (var i = 0; i != separation; i++)
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