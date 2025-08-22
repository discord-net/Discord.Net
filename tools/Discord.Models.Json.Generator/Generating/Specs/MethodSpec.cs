using System.Text;

namespace Discord.Models.Json.Generator.Specs;

public sealed record MethodSpec
{
    public string? Preface { get; init; }
    public string Name { get; init; }
    public string ReturnType { get; init; }
    public Accessibility Accessibility { get; init; }
    public string? ExplicitInterfaceImplementation { get; init; }
    public string? Expression { get; init; }
    public string? Body { get; init; }
    public List<string> Modifiers { get; init; }

    public List<ParameterSpec> Parameters { get; init; }

    public List<GenericSpec> Generics { get; init; }

    public List<GenericConstraintSpec> GenericConstraints { get; init; }

    public MethodSpec(
        string name,
        string returnType,
        Accessibility accessibility = Accessibility.NotApplicable,
        IEnumerable<string>? modifiers = null,
        IEnumerable<ParameterSpec>? parameters = null,
        IEnumerable<GenericSpec>? generics = null,
        IEnumerable<GenericConstraintSpec>? genericConstraints = null,
        string? explicitInterfaceImplementation = null,
        string? expression = null,
        string? body = null,
        string? preface = null)
    {
        Name = name;
        ReturnType = returnType;
        Accessibility = accessibility;
        ExplicitInterfaceImplementation = explicitInterfaceImplementation;
        Expression = expression;
        Body = body;
        Modifiers = [..modifiers ?? []];
        Parameters = [..parameters ?? []];
        Generics = [..generics ?? []];
        GenericConstraints = [..genericConstraints ?? []];
        Preface = preface;
    }
    
    public string ToInvocationString()
    {
        var builder = new StringBuilder();

        builder.Append(Name);

        if (Generics.Count > 0)
        {
            builder.Append('<').Append(string.Join(", ", Generics.Select(x => x.Name))).Append('>');
        }

        builder.Append('(');
        
        if(Parameters.Count > 0)
            builder.Append(string.Join(", ", Parameters.Select(x => x.Name)));

        return builder.Append(')').ToString();
    }
    
    public override string ToString()
    {
        var builder = new StringBuilder();
        
        if (Preface is not null) builder.AppendLine(Preface);
        
        if (ExplicitInterfaceImplementation is null && Accessibility is not Accessibility.NotApplicable)
        {
            builder
                .Append(Accessibility.ToKeywords())
                .Append(' ');
        }

        if (Modifiers.Count > 0)
        {
            builder.Append(string.Join(" ", Modifiers)).Append(' ');
        }

        builder.Append(ReturnType).Append(' ');

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

        if (Parameters.Count > 2)
        {
            var includeParameterDefaults = ExplicitInterfaceImplementation is null;
            
            builder.AppendLine()
                .AppendLine(string
                    .Join(
                        $",{Environment.NewLine}",
                        Parameters.Select(x => x.ToString(includeParameterDefaults))
                    )
                    .Prefix(4)
                    .WithNewlinePadding(4)
                );
        }
        else if (Parameters.Count > 0)
            builder.Append(string.Join(", ", Parameters));

        builder
            .Append(')');

        if (GenericConstraints.Count > 0)
        {
            builder
                .AppendLine()
                .Append(string.Join(Environment.NewLine, GenericConstraints).Prefix(4).WithNewlinePadding(4));
        }

        if (Expression is not null)
        {
            builder.Append($" => {Expression};");
        }
        else if (Body is not null)
        {
            builder.AppendLine()
                .AppendLine("{")
                .AppendLine(Body.Prefix(4).WithNewlinePadding(4).TrimEnd(['\r', '\n', ' ', '\t']))
                .AppendLine("}");
        }
        else
        {
            builder.Append(";");
        }

        return builder.ToString();
    }
}