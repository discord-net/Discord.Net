using System.Text;

namespace Discord.Models.Json.Generator.Specs;

public sealed record IndexerSpec
{
    public List<string> Modifiers { get; init; }

    public List<ParameterSpec> Parameters { get; init; }

    public bool HasAutoGetter => AutoGet is not Accessibility.NotApplicable;
    public bool HasAutoSetter => AutoSet is not Accessibility.NotApplicable;

    public bool HasGetter
        => Getter is not null || HasAutoGetter;

    public bool HasSetter
        => Setter is not null || HasAutoSetter;

    public string Type { get; init; }
    public Accessibility Accessibility { get; init; }
    public string? ExplicitInterfaceImplementation { get; init; }
    public Accessibility AutoGet { get; init; }
    public Accessibility AutoSet { get; init; }
    public string? Getter { get; init; }
    public string? Setter { get; init; }
    public string? Expression { get; init; }
    
    public IndexerSpec(
        string type, 
        IEnumerable<ParameterSpec>? parameters = null,
        Accessibility accessibility = Accessibility.NotApplicable, 
        IEnumerable<string>? modifiers = null,
        string? explicitInterfaceImplementation = null, 
        Accessibility autoGet = Accessibility.Public,
        Accessibility autoSet = Accessibility.NotApplicable, 
        string? getter = null, 
        string? setter = null,
        string? expression = null)
    {
        Type = type;
        Accessibility = accessibility;
        ExplicitInterfaceImplementation = explicitInterfaceImplementation;
        AutoGet = autoGet;
        AutoSet = autoSet;
        Getter = getter;
        Setter = setter;
        Expression = expression;
        Modifiers = [..modifiers ?? []];
        Parameters = [..parameters ?? []];
    }
    

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (Accessibility is not Accessibility.NotApplicable)
            builder.Append(Accessibility.ToKeywords()).Append(' ');

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

        builder
            .Append("this[")
            .Append(string.Join(", ", Parameters.Select(x => x.ToString())))
            .Append(']');

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
                        .Append(AutoGet.ToKeywords())
                        .Append(' ');

                builder.Append("get; ");

                if (HasAutoSetter)
                {
                    if (AutoSet is not Accessibility.Public)
                        builder
                            .Append(AutoSet.ToKeywords())
                            .Append(' ');

                    builder.Append("set; ");
                }

                builder.Append('}');
            }
            else
            {
                builder.AppendLine()
                    .AppendLine("{")
                    .Append(string.Empty.Prefix(4));

                if (HasAutoGetter && AutoGet is not Accessibility.Public)
                    builder
                        .Append(AutoGet.ToKeywords())
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
                        .Append(string.Empty.Prefix(4));

                    if (HasAutoSetter && AutoSet is not Accessibility.Public)
                        builder
                            .Append(AutoSet.ToKeywords())
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