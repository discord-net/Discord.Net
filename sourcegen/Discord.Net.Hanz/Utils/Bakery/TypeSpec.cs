using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Utils.Bakery;

public readonly record struct TypeSpec(
    string Name,
    TypeKind Kind
)
{
    public ImmutableEquatableArray<string> Bases { get; init; }
    public Accessibility Accessibility { get; init; } = Accessibility.Public;
    public ImmutableEquatableArray<string> Modifiers { get; init; } = ImmutableEquatableArray<string>.Empty;

    public ImmutableEquatableArray<TypeSpec> Children { get; init; } = ImmutableEquatableArray<TypeSpec>.Empty;

    public ImmutableEquatableArray<PropertySpec> Properties { get; init; } =
        ImmutableEquatableArray<PropertySpec>.Empty;

    public ImmutableEquatableArray<FieldSpec> Fields { get; init; }
        = ImmutableEquatableArray<FieldSpec>.Empty;

    public ImmutableEquatableArray<ConstructorSpec> Constructors { get; init; }
        = ImmutableEquatableArray<ConstructorSpec>.Empty;

    public ImmutableEquatableArray<GenericSpec> Generics { get; init; } = ImmutableEquatableArray<GenericSpec>.Empty;

    public ImmutableEquatableArray<GenericConstraintSpec> GenericConstraints { get; init; }
        = ImmutableEquatableArray<GenericConstraintSpec>.Empty;

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

        builder.Append(Name);

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
                .Append(string.Join($",{Environment.NewLine}", Bases).WithNewlinePadding(4));
        }

        if (GenericConstraints.Count > 0)
        {
            builder
                .AppendLine()
                .AppendLine(string.Join(Environment.NewLine, GenericConstraints));
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
                    Environment.NewLine,
                    Children.Select(x => x
                        .ToString()
                        .PrefixSpaces(4)
                        .WithNewlinePadding(4)
                    )
                )
            );

            // foreach (var type in Children)
            // {
            //     builder
            //         .AppendLine(type.ToString().PrefixSpaces(4).WithNewlinePadding(4))
            //         .AppendLine();
            // }
        }

        return builder.ToString();
    }
}

public readonly record struct ConstructorSpec(
    string Name,
    Accessibility Accessibility
)
{
    public ImmutableEquatableArray<ParameterSpec> Parameters { get; init; }

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
    string Name
)
{
    public string? Default { get; init; }
}

public readonly record struct GenericSpec(
    string Name
)
{
    public VarianceKind Variance { get; init; } = VarianceKind.None;

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

public readonly record struct PropertySpec(
    string Name,
    string Type,
    Accessibility Accessibility
)
{
    public ImmutableEquatableArray<string> Modifiers { get; init; } = ImmutableEquatableArray<string>.Empty;

    public string? ExplicitInterfaceImplementation { get; init; }

    public Accessibility AutoGet { get; init; } = Accessibility.Public;
    public Accessibility AutoSet { get; init; } = Accessibility.NotApplicable;

    public string? Getter { get; init; }
    public string? Setter { get; init; }

    public bool HasAutoGetter => AutoGet is not Accessibility.Public and not Accessibility.NotApplicable;
    public bool HasAutoSetter => AutoSet is not Accessibility.Public and not Accessibility.NotApplicable;

    public bool HasGetter
        => Getter is not null || HasAutoGetter;

    public bool HasSetter
        => Setter is not null || HasAutoSetter;

    public string? Expression { get; init; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (ExplicitInterfaceImplementation is not null)
        {
            builder
                .Append(SyntaxFacts.GetText(Accessibility))
                .Append(' ');
        }

        if (Modifiers.Count > 0)
        {
            builder
                .Append(string.Join(" ", Modifiers))
                .Append(' ');
        }

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

        return builder.ToString();
    }
}