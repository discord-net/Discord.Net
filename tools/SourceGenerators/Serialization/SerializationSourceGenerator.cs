using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.SourceGenerators.Serialization
{
    [Generator]
    public partial class SerializationSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                "build_property.DiscordNet_SerializationGenerator_OptionsTypeNamespace",
                out var serializerOptionsNamespace))
                throw new InvalidOperationException(
                    "Missing output namespace. Set DiscordNet_SerializationGenerator_OptionsTypeNamespace in your project file.");

            bool searchThroughReferencedAssemblies =
                context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
                    "build_property.DiscordNet_SerializationGenerator_SearchThroughReferencedAssemblies",
                    out var _);

            var generateSerializerAttribute = context.Compilation
                .GetTypeByMetadataName(
                    "Discord.Net.Serialization.GenerateSerializerAttribute");
            var discriminatedUnionSymbol = context.Compilation
                .GetTypeByMetadataName(
                    "Discord.Net.Serialization.DiscriminatedUnionAttribute");
            var discriminatedUnionMemberSymbol = context.Compilation
                .GetTypeByMetadataName(
                    "Discord.Net.Serialization.DiscriminatedUnionMemberAttribute");

            Debug.Assert(generateSerializerAttribute != null);
            Debug.Assert(discriminatedUnionSymbol != null);
            Debug.Assert(discriminatedUnionMemberSymbol != null);
            Debug.Assert(context.SyntaxContextReceiver != null);

            var receiver = (SyntaxReceiver)context.SyntaxContextReceiver!;
            var symbolsToBuild = receiver.GetSerializedTypes(
                context.Compilation);

            if (searchThroughReferencedAssemblies)
            {
                var visitor = new VisibleTypeVisitor(context.CancellationToken);
                foreach (var module in context.Compilation.Assembly.Modules)
                    foreach (var reference in module.ReferencedAssemblySymbols)
                        visitor.Visit(reference);

                symbolsToBuild = symbolsToBuild
                    .Concat(visitor.GetVisibleTypes());
            }

            var types = SerializedTypeUtils.BuildTypeTrees(
                generateSerializerAttribute: generateSerializerAttribute!,
                discriminatedUnionSymbol: discriminatedUnionSymbol!,
                discriminatedUnionMemberSymbol: discriminatedUnionMemberSymbol!,
                symbolsToBuild: symbolsToBuild);

            foreach (var type in types)
            {
                context.AddSource($"Converters.{type.ConverterTypeName}",
                    type.GenerateSourceCode(serializerOptionsNamespace));

                if (type is DiscriminatedUnionSerializedType duDeclaration)
                    foreach (var member in duDeclaration.Members)
                        context.AddSource(
                            $"Converters.{type.ConverterTypeName}.{member.ConverterTypeName}",
                            member.GenerateSourceCode(serializerOptionsNamespace));
            }

            context.AddSource("SerializerOptions",
                GenerateSerializerOptionsSourceCode(
                    serializerOptionsNamespace, types));
        }

        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(
                () => new SyntaxReceiver());

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            private readonly List<SyntaxNode> _classes;

            public SyntaxReceiver()
            {
                _classes = new();
            }

            public IEnumerable<INamedTypeSymbol> GetSerializedTypes(
                Compilation compilation)
            {
                foreach (var @class in _classes)
                {
                    var semanticModel = compilation.GetSemanticModel(
                        @class.SyntaxTree);

                    if (semanticModel.GetDeclaredSymbol(@class) is
                        INamedTypeSymbol classSymbol)
                        yield return classSymbol;
                }
            }

            private INamedTypeSymbol? _generateSerializerAttributeSymbol;

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                _generateSerializerAttributeSymbol ??=
                    context.SemanticModel.Compilation.GetTypeByMetadataName(
                        "Discord.Net.Serialization.GenerateSerializerAttribute");

                Debug.Assert(_generateSerializerAttributeSymbol != null);

                if (context.Node is ClassDeclarationSyntax classDeclaration
                    && classDeclaration.AttributeLists is
                        SyntaxList<AttributeListSyntax> classAttributeLists
                    && classAttributeLists.Any(
                        list => list.Attributes.Any(
                            n => IsAttribute(n, context.SemanticModel,
                                _generateSerializerAttributeSymbol!))))
                {
                    _classes.Add(classDeclaration);
                }
                else if (context.Node is RecordDeclarationSyntax recordDeclaration
                    && recordDeclaration.AttributeLists is
                        SyntaxList<AttributeListSyntax> recordAttributeLists
                    && recordAttributeLists.Any(
                        list => list.Attributes.Any(
                            n => IsAttribute(n, context.SemanticModel,
                                _generateSerializerAttributeSymbol!))))
                {
                    _classes.Add(recordDeclaration);
                }

                static bool IsAttribute(AttributeSyntax attribute,
                    SemanticModel model, INamedTypeSymbol expected)
                {
                    var typeInfo = model.GetTypeInfo(attribute.Name);

                    return SymbolEqualityComparer.Default.Equals(
                        typeInfo.Type, expected);
                }
            }
        }
    }
}
