using System.Text;

namespace Discord.Models.Json.Generator.Specs;

public sealed record OperatorSpec
{
    public string OperatorKind { get; init; }
    public List<ParameterSpec> Parameters { get; init; }
    public Accessibility Accessibility { get; init; }
    public bool Abstract { get; init; }
    public bool Implicit { get; init; }
    public bool Explicit { get; init; }
    public string? Expression { get; init; }
    public string? Body { get; init; }
    public string? ReturnType { get; init; }
    
    public OperatorSpec(
        string operatorKind, 
        IEnumerable<ParameterSpec> parameters,
        Accessibility accessibility = Accessibility.Public, 
        bool @abstract = false,
        bool @implicit = false,
        bool @explicit = false, 
        string? expression = null, 
        string? body = null, 
        string? returnType = null)
    {
        OperatorKind = operatorKind;
        Parameters = [..parameters];
        Accessibility = accessibility;
        Abstract = @abstract;
        Implicit = @implicit;
        Explicit = @explicit;
        Expression = expression;
        Body = body;
        ReturnType = returnType;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Accessibility.ToKeywords())
            .Append(' ')
            .Append("static ");
        
        if(Abstract) sb.Append("abstract ");

        if (Implicit)
            sb.Append("implicit ");
        else if(Explicit)
            sb.Append("explicit ");
        else if (ReturnType is null)
            throw new InvalidOperationException();
        else
            sb.Append(ReturnType).Append(' ');

        sb.Append("operator ").Append(OperatorKind).Append('(');

        foreach (var param in Parameters)
        {
            sb.Append(param).Append(", ");
        }

        sb.Length -= 2;
        
        sb.Append(")");

        if (Body is not null)
        {
            sb.AppendLine()
                .AppendLine("{")
                .Append("".Prefix(4)).AppendLine(Body.WithNewlinePadding(4))
                .AppendLine("}");
        }
        else if (Expression is not null)
        {
            sb.Append(" => ").Append(Expression).Append(';');
        }
        else throw new InvalidOperationException();

        return sb.ToString();
    }
    
}