using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public static class EntityHierarchies
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        List<TypeOfExpressionSyntax> targetInterfaces
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public List<TypeOfExpressionSyntax> TargetInterfaces { get; } = targetInterfaces;
    }

    public static GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax target) return null;

        foreach (var attribute in target.AttributeLists.SelectMany(x => x.Attributes))
        {
            if(context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol) continue;

            var attributeName = attributeSymbol.ContainingType.ToDisplayString();

            if (attributeName == "Discord.ExtendInterfaceDefaultsAttribute" && attribute.ArgumentList is not null)
            {
                var targetTypes = attribute.ArgumentList.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList();

                if (targetTypes.Count == 0)
                {
                    // TODO: diag
                    continue;
                }

                return new GenerationTarget(
                    context.SemanticModel,
                    target,
                    targetTypes
                );

            }
        }

        return null;
    }


    public static void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if(target is null) return;

        foreach (var typeofInterface in target.TargetInterfaces)
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

            var declarations = new List<string>();

            foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>().Where(x => x.IsVirtual))
            {
                var parameterIdentifiers = string.Join(
                    ", ",
                    method.Parameters.Select(x => x.Name)
                );

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

                declarations.Add(
                    $"public {Generics.ResolveGeneric(method.ReturnType, typeSymbol, semanticInterface)} {method.Name}({string.Join(", ", parameterSymbols)})" +
                    $" => (this as {semanticInterface.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}).{method.Name}({parameterIdentifiers});"
                );
            }

            if (declarations.Count == 0)
            {
                return;
            }

            context.AddSource(
                $"EntityHierarchies/{target.ClassDeclarationSyntax.Identifier}_{semanticInterface.Type.Name}",
                $$"""
                  namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  #nullable enable

                  public partial class {{target.ClassDeclarationSyntax.Identifier}}
                  {
                      {{string.Join("\n    ", declarations)}}
                  }

                  #nullable restore
                  """
                );
        }
    }
}
