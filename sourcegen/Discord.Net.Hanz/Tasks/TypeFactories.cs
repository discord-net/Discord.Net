using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public static class TypeFactories
{
    public class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        ParameterListSyntax? primaryConstructorParameters,
        List<ConstructorDeclarationSyntax> constructorDeclarationSyntax
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public ParameterListSyntax? PrimaryConstructorParameters { get; } = primaryConstructorParameters;
        public List<ConstructorDeclarationSyntax> ConstructorDeclarationSyntax { get; } = constructorDeclarationSyntax;
    }

    public static bool IsTarget(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax cls) return false;
        return cls.AttributeLists.Count > 0 || cls.Members.Any(x =>
            x is ConstructorDeclarationSyntax {AttributeLists.Count: > 0}
        );
    }

    public static GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax target)
            return null;

        ParameterListSyntax? primaryConstructorParameters = null;
        List<ConstructorDeclarationSyntax> constructors = new();

        // primary constructor
        foreach (var attribute in target.AttributeLists.SelectMany(x => x.Attributes))
        {
            if (Attributes.GetAttributeName(attribute, context.SemanticModel) != "Discord.TypeFactoryAttribute")
                continue;

            var parameters = target.ChildNodes().OfType<ParameterListSyntax>().FirstOrDefault();

            if (parameters is null)
            {
                Hanz.Logger.Warn("Found type factory with no primary constructor");
                continue;
            }

            primaryConstructorParameters = parameters;
            break;
        }

        foreach (var constructor in target.Members.OfType<ConstructorDeclarationSyntax>())
        {
            foreach (var attribute in constructor.AttributeLists.SelectMany(x => x.Attributes))
            {
                if (Attributes.GetAttributeName(attribute, context.SemanticModel) != "Discord.TypeFactoryAttribute")
                    continue;

                constructors.Add(constructor);
            }
        }

        if (primaryConstructorParameters is not null || constructors.Count > 0)
            return new GenerationTarget(context.SemanticModel, target, primaryConstructorParameters, constructors);

        return null;
    }

    public static void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target is null) return;

        var sb = new StringBuilder();

        if (target.PrimaryConstructorParameters is not null)
        {
            AppendFactory(sb, target.ClassDeclarationSyntax.Identifier.ValueText, target.PrimaryConstructorParameters);
        }

        foreach (var constructor in target.ConstructorDeclarationSyntax)
        {
            AppendFactory(sb, target.ClassDeclarationSyntax.Identifier.ValueText, constructor.ParameterList);
        }

        context.AddSource(
            $"Factories/{target.ClassDeclarationSyntax.Identifier}_Factory",
            $$"""
            namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

            public partial class {{target.ClassDeclarationSyntax.Identifier}}
            {
                {{sb.ToString().Replace("\n", "\n    ")}}
            }
            """
        );
    }

    private static void AppendFactory(StringBuilder builder, string name, ParameterListSyntax parameterList)
    {
        var parameterNames = string.Join(", ", parameterList.Parameters.Select(x => x.Identifier));
        builder.AppendLine(
            $"internal static {name} Factory{parameterList.NormalizeWhitespace()} => new({parameterNames});"
        );

        foreach (var defaultParam in parameterList.Parameters.Where(x => x.Default is not null))
        {
            var newList = parameterList.RemoveNode(defaultParam, SyntaxRemoveOptions.KeepNoTrivia) ?? SyntaxFactory.ParameterList([]);
            parameterNames = string.Join(", ", newList.Parameters.Select(x => x.Identifier));
            builder.AppendLine(
                $"internal static {name} Factory{newList.NormalizeWhitespace()} => new({parameterNames});"
            );
        }
    }
}
