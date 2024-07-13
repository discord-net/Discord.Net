using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public class TypeFactories : IGenerationTask<TypeFactories.GenerationTarget>
{
    public class ConstructorArgs(ParameterListSyntax parameters, string? shouldBeLastParameter)
    {
        public ParameterListSyntax Parameters { get; } = parameters;
        public string? ShouldBeLastParameter { get; } = shouldBeLastParameter;
    }

    public class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        ConstructorArgs? primaryConstructorParameters,
        List<ConstructorArgs> constructorDeclarationSyntax
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public ConstructorArgs? PrimaryConstructorParameters { get; } = primaryConstructorParameters;
        public List<ConstructorArgs> ConstructorDeclarationSyntax { get; } = constructorDeclarationSyntax;
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        if (node is not ClassDeclarationSyntax cls) return false;
        return cls.AttributeLists.Count > 0 || cls.Members.Any(x =>
            x is ConstructorDeclarationSyntax {AttributeLists.Count: > 0}
        );
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not ClassDeclarationSyntax target)
            return null;

        ConstructorArgs? primaryConstructorParameters = null;
        List<ConstructorArgs> constructors = new();

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

            primaryConstructorParameters = new ConstructorArgs(
                parameters,
                Attributes.GetAttributeNamedNameOfArg(attribute, "LastParameter")
            );
            break;
        }

        foreach (var constructor in target.Members.OfType<ConstructorDeclarationSyntax>())
        {
            foreach (var attribute in constructor.AttributeLists.SelectMany(x => x.Attributes))
            {
                if (Attributes.GetAttributeName(attribute, context.SemanticModel) != "Discord.TypeFactoryAttribute")
                    continue;

                constructors.Add(new ConstructorArgs(
                    constructor.ParameterList,
                    Attributes.GetAttributeNamedNameOfArg(attribute, "LastParameter")
                ));
            }
        }

        if (primaryConstructorParameters is not null || constructors.Count > 0)
            return new GenerationTarget(context.SemanticModel, target, primaryConstructorParameters, constructors);

        return null;
    }

    public void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target is null) return;

        var sb = new StringBuilder();

        if (target.PrimaryConstructorParameters is not null)
        {
            AppendFactory(sb, target.ClassDeclarationSyntax.Identifier.ValueText, target.PrimaryConstructorParameters);
        }

        foreach (var constructor in target.ConstructorDeclarationSyntax)
        {
            AppendFactory(sb, target.ClassDeclarationSyntax.Identifier.ValueText, constructor);
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

    private static void AppendFactory(StringBuilder builder, string name, ConstructorArgs args)
    {
        var parameterList = args.Parameters;
        var parameterNames = string.Join(", ", parameterList.Parameters.Select(x => x.Identifier));

        builder.AppendLine(
            $"internal static {name} Factory{ReorderToLast(RemoveParameterDefaults(parameterList), args.ShouldBeLastParameter).NormalizeWhitespace()} => new({parameterNames});"
        );

        foreach (var defaultParam in parameterList.Parameters.Where(x => x.Default is not null))
        {
            var newList = RemoveParameterDefaults(parameterList.RemoveNode(defaultParam, SyntaxRemoveOptions.KeepNoTrivia) ?? SyntaxFactory.ParameterList([]));
            parameterNames = string.Join(", ", newList.Parameters.Select(x => x.Identifier));
            builder.AppendLine(
                $"internal static {name} Factory{ReorderToLast(newList, args.ShouldBeLastParameter).NormalizeWhitespace()} => new({parameterNames});"
            );
        }
    }

    private static ParameterListSyntax ReorderToLast(ParameterListSyntax list, string? shouldBeLast)
    {
        if (shouldBeLast is null) return list;

        var param = list.Parameters.FirstOrDefault(x => x.Identifier.ValueText == shouldBeLast);

        if (param is null)
            return list;

        return list
            .RemoveNode(param, SyntaxRemoveOptions.KeepNoTrivia)!
            .AddParameters(param);
    }

    private static ParameterListSyntax RemoveParameterDefaults(ParameterListSyntax list)
    {
        var newList = list;

        foreach (var parameter in list.Parameters)
        {
            if (parameter.Default is not null)
            {
                newList = newList.ReplaceNode(
                    parameter,
                    parameter.WithDefault(null)
                );
            }
        }

        return newList;
    }
}
