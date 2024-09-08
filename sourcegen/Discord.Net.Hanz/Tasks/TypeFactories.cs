using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public class TypeFactories : ISyntaxGenerationCombineTask<TypeFactories.GenerationTarget>
{
    public class ConstructorArgs(ParameterListSyntax parameters, string? shouldBeLastParameter)
        : IEquatable<ConstructorArgs>
    {
        public ParameterListSyntax Parameters { get; } = parameters;
        public string? ShouldBeLastParameter { get; } = shouldBeLastParameter;

        public bool Equals(ConstructorArgs? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Parameters.IsEquivalentTo(other.Parameters) && ShouldBeLastParameter == other.ShouldBeLastParameter;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ConstructorArgs)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Parameters.GetHashCode() * 397) ^
                       (ShouldBeLastParameter != null ? ShouldBeLastParameter.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ConstructorArgs? left, ConstructorArgs? right) => Equals(left, right);

        public static bool operator !=(ConstructorArgs? left, ConstructorArgs? right) => !Equals(left, right);
    }

    public class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        ConstructorArgs? primaryConstructorParameters,
        List<ConstructorArgs> constructorDeclarationSyntax
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public ConstructorArgs? PrimaryConstructorParameters { get; } = primaryConstructorParameters;
        public List<ConstructorArgs> ConstructorDeclarationSyntax { get; } = constructorDeclarationSyntax;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassDeclarationSyntax.IsEquivalentTo(other.ClassDeclarationSyntax) &&
                   (PrimaryConstructorParameters?.Equals(other.PrimaryConstructorParameters) ??
                    other.PrimaryConstructorParameters is not null) &&
                   ConstructorDeclarationSyntax.SequenceEqual(other.ConstructorDeclarationSyntax);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GenerationTarget)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ClassDeclarationSyntax.GetHashCode();
                hashCode = (hashCode * 397) ^ (PrimaryConstructorParameters != null
                    ? PrimaryConstructorParameters.GetHashCode()
                    : 0);
                hashCode = (hashCode * 397) ^ ConstructorDeclarationSyntax.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        if (node is not ClassDeclarationSyntax cls) return false;
        return cls.AttributeLists.Count > 0 || cls.Members.Any(x =>
            x is ConstructorDeclarationSyntax {AttributeLists.Count: > 0}
        );
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token)
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
                logger.Warn("Found type factory with no primary constructor");
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

    private readonly struct FactoryDetails(string[] arguments) : IEquatable<FactoryDetails>
    {
        public readonly string[] Arguments = arguments;

        public bool Equals(FactoryDetails other) => Arguments.Length == other.Arguments.Length;

        public override bool Equals(object? obj) => obj is FactoryDetails other && Equals(other);

        public override int GetHashCode() => Arguments.Length;

        public static bool operator ==(FactoryDetails left, FactoryDetails right) => left.Equals(right);

        public static bool operator !=(FactoryDetails left, FactoryDetails right) => !left.Equals(right);
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        var factoryArgsList = new HashSet<int>();
        var generated = new HashSet<string>();

        foreach (var target in targets)
        {
            if (target is null) continue;

            if(!generated.Add(target.ClassDeclarationSyntax.Identifier.ValueText))
                continue;

            var sb = new StringBuilder();

            var implementedFactories = new HashSet<FactoryDetails>();

            if (target.PrimaryConstructorParameters is not null)
            {
                foreach (var details in AppendFactory(sb, target.ClassDeclarationSyntax.Identifier.ValueText,
                             target.PrimaryConstructorParameters))
                    implementedFactories.Add(details);
            }

            foreach (var constructor in target.ConstructorDeclarationSyntax)
            {
                foreach (var details in AppendFactory(sb, target.ClassDeclarationSyntax.Identifier.ValueText,
                             constructor))
                    implementedFactories.Add(details);
            }


            var bases = implementedFactories.Select(x =>
                $"IFactory<{target.ClassDeclarationSyntax.Identifier}, {string.Join(", ", x.Arguments)}>"
            ).ToArray();

            var modifiers = string.Join(" ", target.ClassDeclarationSyntax.Modifiers);

            context.AddSource(
                $"Factories/{target.ClassDeclarationSyntax.Identifier}",
                $$"""
                  namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  {{modifiers}} class {{target.ClassDeclarationSyntax.Identifier}}{{(bases.Length > 0 ? $" : {string.Join(", ", bases)}" : "")}}
                  {
                      {{sb.ToString().Replace("\n", "\n    ")}}

                      {{string.Join("\n    ", bases.Select((x, i) =>
                          $$"""
                            static {{target.ClassDeclarationSyntax.Identifier}} {{x}}.Factory({{string.Join(", ", implementedFactories.ElementAt(i).Arguments.Select((x, i) => $"{x} arg{i}"))}})
                                    => Factory({{string.Join(", ", implementedFactories.ElementAt(i).Arguments.Select((x, i) => $"arg{i}"))}});
                            """
                      ))}}
                  }
                  """
            );

            foreach (var implemented in implementedFactories)
                factoryArgsList.Add(implemented.Arguments.Length);
        }

        if (factoryArgsList.Count > 0)
        {
            var mapped = factoryArgsList.Select(x =>
                $$"""
                  internal interface IFactory<TSelf, {{string.Join(", ", Enumerable.Range(0, x).Select(x => $"T{x}"))}}>
                  {
                      static abstract TSelf Factory({{string.Join(", ", Enumerable.Range(0, x).Select(x => $"T{x} arg{x}"))}});
                  }
                  """
            );

            context.AddSource(
                "Factories/IFactory",
                $$"""
                  namespace Discord;

                  {{string.Join("\n\n", mapped)}}
                  """
            );
        }
    }

    private static IEnumerable<FactoryDetails> AppendFactory(StringBuilder builder, string name, ConstructorArgs args)
    {
        var parameterList = args.Parameters;
        var parameterNames = string.Join(", ", parameterList.Parameters.Select(x => x.Identifier));

        var startList = ReorderToLast(RemoveParameterDefaults(parameterList), args.ShouldBeLastParameter)
            .NormalizeWhitespace();
        builder.AppendLine(
            $"internal static {name} Factory{startList} => new({parameterNames});"
        );

        yield return new FactoryDetails(startList.Parameters.Select(x => x.Type!.ToString()).ToArray());

        var nonOptionalList = parameterList.RemoveNodes(
            parameterList.Parameters.Where(x => x.Default is not null),
            SyntaxRemoveOptions.KeepNoTrivia
        )!;

        for (int i = 0; i != parameterList.Parameters.Count(x => x.Default is not null); i++)
        {
            var optionals = parameterList.Parameters
                .Where(x => x.Default is not null)
                .Take(i)
                .ToArray();

            var newList = ReorderToLast(RemoveParameterDefaults(nonOptionalList.AddParameters(optionals)),
                args.ShouldBeLastParameter);

            yield return new FactoryDetails(newList.Parameters.Select(x => x.Type!.ToString()).ToArray());

            parameterNames = string.Join(
                ", ",
                newList.Parameters
                    .Select(x =>
                    {
                        var original =
                            parameterList.Parameters.First(y => y.Identifier.ValueText == x.Identifier.ValueText);
                        return (Original: original, New: x);
                    })
                    .OrderBy(x => x.Original.Default is not null ? 1 : -1)
                    .Select(x =>
                    {
                        if (x.Original.Default is not null)
                            return $"{x.New.Identifier}: {x.New.Identifier}";

                        return x.New.Identifier.ValueText;
                    })
            );
            builder.AppendLine(
                $"internal static {name} Factory{newList.NormalizeWhitespace()} => new({parameterNames});"
            );
        }

        // foreach (var defaultParam in parameterList.Parameters.Where(x => x.Default is not null))
        // {
        //     var newList =
        //         RemoveParameterDefaults(parameterList.RemoveNode(defaultParam, SyntaxRemoveOptions.KeepNoTrivia) ??
        //                                 SyntaxFactory.ParameterList([]));
        //
        //     yield return new FactoryDetails(newList.Parameters.Select(x => x.Type!.ToString()).ToArray());
        //
        //     parameterNames = string.Join(", ", newList.Parameters.Select(x =>
        //     {
        //         var original =
        //             parameterList.Parameters.First(y => y.Identifier.ValueText == x.Identifier.ValueText);
        //
        //         if (original.Default is not null)
        //             return $"{x.Identifier}: {x.Identifier}";
        //
        //         return x.Identifier.ValueText;
        //     }));
        //     builder.AppendLine(
        //         $"internal static {name} Factory{ReorderToLast(newList, args.ShouldBeLastParameter).NormalizeWhitespace()} => new({parameterNames});"
        //     );
        // }
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
        return list.RemoveNodes(
            list.Parameters
                .Select(x => x.Default)
                .Where(x => x is not null)
                .Cast<EqualsValueClauseSyntax>(),
            SyntaxRemoveOptions.KeepNoTrivia
        )!;
    }
}
