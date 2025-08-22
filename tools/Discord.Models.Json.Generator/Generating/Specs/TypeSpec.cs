using System.Text;

namespace Discord.Models.Json.Generator.Specs;

public sealed class TypeSpec(
    string name,
    string kind,
    Accessibility accessibility = Accessibility.Public,
    IEnumerable<string>? bases = null,
    IEnumerable<string>? modifiers = null,
    IEnumerable<TypeSpec>? children = null,
    IEnumerable<PropertySpec>? properties = null,
    IEnumerable<IndexerSpec>? indexers = null,
    IEnumerable<FieldSpec>? fields = null,
    IEnumerable<ConstructorSpec>? constructors = null,
    IEnumerable<MethodSpec>? methods = null,
    IEnumerable<GenericSpec>? generics = null,
    IEnumerable<GenericConstraintSpec>? genericConstraints = null,
    IEnumerable<ParameterSpec>? parameters = null,
    IEnumerable<OperatorSpec>? operators = null,
    IEnumerable<AttributeSpec>? attributes = null,
    bool record = false
)
{
    public string Name { get; init; } = name;
    public string Kind { get; init; } = kind;
    public bool Record { get; init; } = record;

    public Accessibility Accessibility { get; init; } = accessibility;

    public List<OperatorSpec> Operators { get; init; } = [..operators ?? []];

    public List<ParameterSpec> Parameters { get; init; } = [..parameters ?? []];

    public List<string> Bases { get; init; } = [..bases ?? []];

    public HashSet<string> Modifiers { get; init; } = [..modifiers ?? []];

    public List<TypeSpec> Children { get; init; } = [..children ?? []];

    public List<PropertySpec> Properties { get; init; } = [..properties ?? []];

    public List<IndexerSpec> Indexers { get; init; } = [..indexers ?? []];

    public List<FieldSpec> Fields { get; init; } = [..fields ?? []];

    public List<ConstructorSpec> Constructors { get; init; } = [..constructors ?? []];

    public List<MethodSpec> Methods { get; init; } = [..methods ?? []];

    public List<GenericSpec> Generics { get; init; } = [..generics ?? []];

    public HashSet<GenericConstraintSpec> GenericConstraints { get; init; }
        = [..genericConstraints ?? []];
    
    public List<AttributeSpec> Attributes { get; init; } = [..attributes ?? []];

    public bool HasBrackets
        => Children.Count > 0 ||
           Properties.Count > 0 ||
           Methods.Count > 0 ||
           Fields.Count > 0 ||
           Indexers.Count > 0 ||
           Operators.Count > 0;
    
    public override string ToString()
    {
        var builder = new StringBuilder();

        if (Attributes.Count > 0)
        {
            builder.Append('[');
            foreach (var attribute in Attributes)
            {
                builder.Append(attribute).Append(", ");
            }

            builder.Length -= 2;
            builder.AppendLine("]");
        }

        builder
            .Append(Accessibility.ToKeywords())
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
                .AppendLine("(")
                .Append(string.Join($",{Environment.NewLine}", Parameters.Select(x => x.ToString().Prefix(4))))
                .AppendLine()
                .Append(')');
        }

        if (Bases.Count > 0)
        {
            builder.Append(" : ");

            if (Bases.Count > 1)
                builder.AppendLine();

            builder.Append(string.Join($",{Environment.NewLine}", Bases.Distinct()).Prefix(Bases.Count > 1 ? 4 : 0).WithNewlinePadding(4));
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
                Operators,
                nameof(Operators),
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
            throw new Exception($"Failed to add {name} members ({typeof(T)})", ex);
        }
    }
}