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
    ImmutableEquatableArray<FieldSpec>? Fields = null,
    ImmutableEquatableArray<ConstructorSpec>? Constructors = null,
    ImmutableEquatableArray<MethodSpec>? Methods = null,
    ImmutableEquatableArray<GenericSpec>? Generics = null,
    ImmutableEquatableArray<GenericConstraintSpec>? GenericConstraints = null
)
{
    public ImmutableEquatableArray<string> Bases { get; init; }
        = Bases ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<string> Modifiers { get; init; }
        = Modifiers ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<TypeSpec> Children { get; init; }
        = Children ?? ImmutableEquatableArray<TypeSpec>.Empty;

    public ImmutableEquatableArray<PropertySpec> Properties { get; init; }
        = Properties ?? ImmutableEquatableArray<PropertySpec>.Empty;

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
        => this with {Bases = Bases.AddRange(bases)};
    public TypeSpec AddBases(IEnumerable<string> bases)
        => this with {Bases = Bases.AddRange(bases)};

    public TypeSpec AddProperty(
        PropertySpec property
    ) => this with
    {
        Properties = Properties.Add(property)
    };

    public TypeSpec AddNestedType(
        TypeSpec type
    ) => this with
    {
        Children = Children.Add(type)
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

    public TypeSpec AddInterfaceOverloadMethod(
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
            Kind: typeref.TypeKind
        )
        {
            Accessibility = typeref.Accessibility
        };
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

            foreach
            (
                var property
                in Properties
                    .Where(x => x.ExplicitInterfaceImplementation is null)
                    .OrderByDescending(x => x.Accessibility)
            )
            {
                builder.AppendLine(property.ToString().PrefixSpaces(4).WithNewlinePadding(4)).AppendLine();
            }

            foreach (var field in Fields)
            {
                builder.AppendLine(field.ToString().PrefixSpaces(4).WithNewlinePadding(4));
            }

            foreach (var constructor in Constructors)
            {
                builder.AppendLine(constructor.ToString().PrefixSpaces(4).WithNewlinePadding(4));
            }

            builder.AppendLine(string
                .Join(
                    $"{Environment.NewLine}{Environment.NewLine}",
                    Methods
                )
                .PrefixSpaces(4)
                .WithNewlinePadding(4)
            );

            foreach
            (
                var property
                in Properties
                    .Where(x => x.ExplicitInterfaceImplementation is not null)
                    .OrderByDescending(x => x.Accessibility)
            )
            {
                builder.AppendLine(property.ToString().PrefixSpaces(4).WithNewlinePadding(4));
            }

            builder.AppendLine(string
                .Join(
                    $"{Environment.NewLine}{Environment.NewLine}",
                    Children
                )
                .PrefixSpaces(4)
                .WithNewlinePadding(4)
            );

            builder.Append('}');
        }
        else builder.Append(';');

        return builder.ToString();
    }
}

public readonly record struct ConstructorSpec(
    string Name,
    Accessibility Accessibility
)
{
    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; } =
        ImmutableEquatableArray<ParameterSpec>.Empty;

    public string? BaseInvocation { get; init; }
    public string? Body { get; init; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(SyntaxFacts.GetText(Accessibility))
            .Append(' ')
            .Append(Name)
            .Append('(');

        if (Parameters.Count > 0)
        {
            builder.AppendLine()
                .AppendLine(string
                    .Join(
                        $",{Environment.NewLine}",
                        Parameters
                    )
                    .PrefixSpaces(4)
                    .WithNewlinePadding(4)
                );
        }

        builder
            .Append(')');

        if (BaseInvocation is not null)
            builder.Append(" : ").AppendLine(BaseInvocation);

        builder.AppendLine("{");

        if (Body is not null)
            builder.AppendLine(Body.WithNewlinePadding(4));

        return builder.Append("}").ToString();
    }
}

public readonly record struct ParameterSpec(
    string Type,
    string Name,
    string? Default = null
)
{
    public override string ToString()
        => $"{Type} {Name}{(Default is not null ? $" = {Default}" : string.Empty)}";

    public static implicit operator ParameterSpec((string, string) tuple) => new(tuple.Item1, tuple.Item2);

    public static implicit operator ParameterSpec((string, string, string) tuple) =>
        new(tuple.Item1, tuple.Item2, tuple.Item3);
}

public readonly record struct GenericSpec(
    string Name
)
{
    public VarianceKind Variance { get; init; } = VarianceKind.None;

    public override string ToString()
        => $"{(Variance is not VarianceKind.None ? $"{Variance.ToString().ToLower()} " : string.Empty)}{Name}";

    public static implicit operator GenericSpec(string str) => new(str);
}

public readonly record struct GenericConstraintSpec(
    string Name
)
{
    public ImmutableEquatableArray<string> Constraints { get; init; } = ImmutableEquatableArray<string>.Empty;

    public override string ToString()
    {
        if (Constraints.Count == 0)
            return string.Empty;

        return $"where {Name} : {string.Join(", ", Constraints)}";
    }

    public static implicit operator GenericConstraintSpec((string, string[]) tuple) => new(tuple.Item1)
    {
        Constraints = new(tuple.Item2)
    };
}

public readonly record struct FieldSpec(
    string Name,
    string Type,
    Accessibility Accessibility
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; } = ImmutableEquatableArray<string>.Empty;

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(SyntaxFacts.GetText(Accessibility))
            .Append(' ');

        if (Modifiers.Count > 0)
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');

        builder.Append(Name).Append(';');

        return builder.ToString();
    }
}

public readonly record struct MethodSpec(
    string Name,
    string ReturnType,
    Accessibility Accessibility = Accessibility.NotApplicable,
    ImmutableEquatableArray<string>? Modifiers = null,
    ImmutableEquatableArray<ParameterSpec>? Parameters = null,
    ImmutableEquatableArray<GenericSpec>? Generics = null,
    ImmutableEquatableArray<GenericConstraintSpec>? GenericConstraints = null,
    string? ExplicitInterfaceImplementation = null,
    string? Expression = null,
    string? Body = null
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; }
        = Modifiers ?? ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; }
        = Parameters ?? ImmutableEquatableArray<ParameterSpec>.Empty;

    public ImmutableEquatableArray<GenericSpec> Generics { get; init; }
        = Generics ?? ImmutableEquatableArray<GenericSpec>.Empty;

    public ImmutableEquatableArray<GenericConstraintSpec> GenericConstraints { get; init; }
        = GenericConstraints ?? ImmutableEquatableArray<GenericConstraintSpec>.Empty;

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append(SyntaxFacts.GetText(Accessibility))
            .Append(' ');

        if (Modifiers.Count > 0)
        {
            builder.Append(string.Join(" ", Modifiers)).Append(' ');
        }

        if (ExplicitInterfaceImplementation is not null)
            builder.Append(ExplicitInterfaceImplementation).Append('.');

        builder.Append(Name);

        if (Generics.Count > 0)
        {
            builder
                .Append('<')
                .Append(string.Join(", ", Generics))
                .Append('>');
        }

        builder.Append('(');

        if (Parameters.Count > 0)
        {
            builder.AppendLine()
                .AppendLine(string
                    .Join(
                        $",{Environment.NewLine}",
                        Parameters
                    )
                    .PrefixSpaces(4)
                    .WithNewlinePadding(4)
                );
        }

        builder
            .Append(')');

        if (GenericConstraints.Count > 0)
        {
            builder
                .AppendLine()
                .Append(string.Join(Environment.NewLine, GenericConstraints).PrefixSpaces(4));
        }

        if (Expression is not null)
        {
            builder.Append($" => {Expression};");
        }
        else if (Body is not null)
        {
            builder.AppendLine()
                .AppendLine("{")
                .AppendLine()
                .AppendLine(Body)
                .AppendLine("}");
        }
        else
        {
            builder.Append(";");
        }

        return builder.ToString();
    }
}

public readonly record struct PropertySpec(
    string Type,
    string Name,
    Accessibility Accessibility = Accessibility.NotApplicable,
    ImmutableEquatableArray<string>? Modifiers = null,
    string? ExplicitInterfaceImplementation = null,
    Accessibility AutoGet = Accessibility.Public,
    Accessibility AutoSet = Accessibility.NotApplicable,
    string? Getter = null,
    string? Setter = null,
    string? Expression = null
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; }
        = Modifiers ?? ImmutableEquatableArray<string>.Empty;

    public bool HasAutoGetter => AutoGet is not Accessibility.NotApplicable;
    public bool HasAutoSetter => AutoSet is not Accessibility.NotApplicable;

    public bool HasGetter
        => Getter is not null || HasAutoGetter;

    public bool HasSetter
        => Setter is not null || HasAutoSetter;

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (Accessibility is not Accessibility.NotApplicable)
            builder.Append(SyntaxFacts.GetText(Accessibility)).Append(' ');

        if (Modifiers.Count > 0)
        {
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');
        }

        builder.Append(Type).Append(' ');

        if (ExplicitInterfaceImplementation is not null)
            builder
                .Append(ExplicitInterfaceImplementation)
                .Append('.');

        builder.Append(Name);

        if (Expression is not null)
        {
            builder
                .Append(" => ")
                .Append(Expression)
                .Append(';');
        }
        else if (HasGetter || HasSetter)
        {
            if (Getter is null && Setter is null)
            {
                builder.Append(" { ");

                if (AutoGet is not Accessibility.Public)
                    builder
                        .Append(SyntaxFacts.GetText(AutoGet))
                        .Append(' ');

                builder.Append("get; ");

                if (HasAutoSetter)
                {
                    if (AutoSet is not Accessibility.Public)
                        builder
                            .Append(SyntaxFacts.GetText(AutoSet))
                            .Append(' ');

                    builder.Append("set; ");
                }

                builder.Append('}');
            }
            else
            {
                builder.AppendLine()
                    .AppendLine("{")
                    .Append(string.Empty.PrefixSpaces(4));

                if (HasAutoGetter && AutoGet is not Accessibility.Public)
                    builder
                        .Append(SyntaxFacts.GetText(AutoGet))
                        .Append(' ');

                builder.Append("get");

                if (Getter is not null)
                {
                    builder.Append(Getter.WithNewlinePadding(4));
                }
                else
                {
                    builder.Append(';');
                }

                if (HasSetter)
                {
                    builder
                        .AppendLine()
                        .Append(string.Empty.PrefixSpaces(4));

                    if (HasAutoSetter && AutoSet is not Accessibility.Public)
                        builder
                            .Append(SyntaxFacts.GetText(AutoSet))
                            .Append(' ');

                    builder.Append("set");

                    if (Setter is not null)
                    {
                        builder.Append(Setter.WithNewlinePadding(4));
                    }
                    else
                    {
                        builder.Append(';');
                    }
                }

                builder
                    .AppendLine()
                    .Append('}');
            }
        }

        return builder.ToString();
    }
}