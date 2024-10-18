using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;

public sealed class Constructor
{
    public string Name { get; }
    public List<ConstructorParamter> Parameters { get; }
    public Constructor? BaseConstructor { get; }
    public Accessibility Accessibility { get; }

    public Constructor(
        string name,
        List<ConstructorParamter> parameters,
        Constructor? baseConstructor = null,
        Accessibility accessibility = Accessibility.Internal)
    {
        Parameters = parameters;
        Name = name;
        BaseConstructor = baseConstructor;
        Accessibility = accessibility;
    }

    public List<ConstructorParamter> GetActualParameters()
    {
        var result = new List<ConstructorParamter>(Parameters);

        var current = BaseConstructor;

        while (current is not null)
        {
            result.AddRange(current.Parameters);

            current = current.BaseConstructor;
        }

        return result
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .ToList();
    }

    public string Format()
    {
        var parameters = GetActualParameters();
        var baseParameters = BaseConstructor?.GetActualParameters();

        return
            $$"""
              {{SyntaxFacts.GetText(Accessibility)}} {{Name}}({{(
                  parameters.Count > 0
                      ? $"{Environment.NewLine}{
                          string.Join(
                              $",{Environment.NewLine}",
                              parameters.Select(x => x.Format())
                          )
                      }".WithNewlinePadding(4) + Environment.NewLine
                      : string.Empty
              )}}){{(
              baseParameters is not null
                  ? $" : base({(
                      baseParameters.Count > 0
                          ? $"{Environment.NewLine}{
                              string.Join(
                                  $",{Environment.NewLine}",
                                  baseParameters.Select(x => x.Name))
                          }".WithNewlinePadding(4)
                          : string.Empty
                  )})"
                  : string.Empty
          )}}
              {
                  {{
                      string.Join(
                          Environment.NewLine,
                          Parameters
                              .Where(x => x.Initializes.HasValue)
                              .Select(x => $"{x.Initializes!.Value.Name} = {x.Name};")
                      ).WithNewlinePadding(4)
                  }}
              }
              """;
    }
}

public readonly struct ConstructorParamter
{
    public readonly string Name;
    public readonly string Type;
    public readonly string? Default;
    public readonly Property? Initializes;

    public ConstructorParamter(string name, string type, string? @default = null, Property? initializes = null)
    {
        Name = name;
        Type = type;
        Default = @default;
        Initializes = initializes;
    }

    public string Format()
    {
        var sb = new StringBuilder($"{Type} {Name}");

        if (Default is not null)
            sb.Append(" = ").Append(Default);

        return sb.ToString();
    }
}