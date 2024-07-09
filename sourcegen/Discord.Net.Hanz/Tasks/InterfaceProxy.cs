using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public static class InterfaceProxy
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> properties)
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> Properties { get; } = properties;
    }

    public static bool IsTarget(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDeclarationSyntax)
            return false;

        return classDeclarationSyntax.Members.Any(x => x is PropertyDeclarationSyntax {AttributeLists.Count: > 0});
    }

    public static GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax target) return null;

        var dict = new Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>>();

        foreach (var member in target.Members
                     .Where(x => x is PropertyDeclarationSyntax{AttributeLists.Count: > 0}))
        {
            if (member is not PropertyDeclarationSyntax property) continue;

            foreach (var attribute in property.AttributeLists.SelectMany(x => x.Attributes))
            {
                if(attribute.ArgumentList is null) continue;

                if(context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol) continue;

                if(attributeSymbol.ContainingType.ToDisplayString() != "Discord.ProxyInterfaceAttribute") continue;

                dict.Add(property, attribute.ArgumentList.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList());
            }
        }

        if (dict.Count > 0)
        {
            return new GenerationTarget(
                context.SemanticModel,
                target,
                dict
            );
        }

        return null;
    }


    public static void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target is null) return;

        var sb = new StringBuilder();

        foreach (var kvp in target.Properties)
        {
            var property = kvp.Key;

            sb.AppendLine($"#region Property {kvp.Key.Identifier}");

            foreach (var typeofInterface in kvp.Value)
            {
                var genericNode = typeofInterface.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();

                var semanticInterface = target.SemanticModel.GetTypeInfo(genericNode ?? typeofInterface.Type);

                if (semanticInterface.Type is null)
                {
                    return;
                }

                if (semanticInterface.Type is not INamedTypeSymbol typeSymbol)
                {
                    return;
                }

                sb.AppendLine($"#region Interface {typeSymbol.Name}");

                foreach (var member in typeSymbol.GetMembers())
                {
                    switch (member)
                    {
                        case IMethodSymbol method:
                            if (method.IsStatic || method.MethodKind is not MethodKind.Ordinary)
                                break;
                            var parameterIdentifiers = method.Parameters.Select(x => x.Name);
                            var parameterSymbols = method.Parameters.Select(x =>
                            {
                                var param = $"{Generics.ResolveGeneric(x.Type, typeSymbol, semanticInterface)} {x.Name}";


                                if (x.HasExplicitDefaultValue)
                                    param += x.ExplicitDefaultValue is null
                                        ? " = default"
                                        : $" = {x.ExplicitDefaultValue.ToString().ToLowerInvariant()}";
                                else if (x.IsOptional)
                                    param += " = default";

                                return param;
                            });
                            sb.AppendLine(
                                $"public {Generics.ResolveGeneric(method.ReturnType, typeSymbol, semanticInterface)} {method.Name}({string.Join(", ", parameterSymbols)}) => ({property.Identifier} as {typeSymbol}).{method.Name}({string.Join(", ", parameterIdentifiers)});"
                            );
                            break;
                        case IPropertySymbol interfaceProperty:
                            if (interfaceProperty.ExplicitInterfaceImplementations.Length > 0)
                                break;
                            sb.AppendLine(
                                $"public {Generics.ResolveGeneric(interfaceProperty.Type, typeSymbol, semanticInterface)} {interfaceProperty.Name} => ({property.Identifier} as {semanticInterface.Type}).{interfaceProperty.Name};"
                            );
                            break;
                    }
                }

                sb.AppendLine("#endregion");
            }

            sb.Append($"#endregion");

            context.AddSource(
                $"InterfaceProxy/{target.ClassDeclarationSyntax.Identifier}_Proxied.g.cs",
                $$"""
                  namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  #nullable enable

                  public partial class {{target.ClassDeclarationSyntax.Identifier}}
                  {
                      {{sb.ToString().Replace("\n", "\n    ")}}
                  }

                  #nullable restore
                  """
            );
        }
    }
}
