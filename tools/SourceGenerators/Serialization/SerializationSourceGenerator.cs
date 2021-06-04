using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.SourceGenerators.Serialization
{
    [Generator]
    public partial class SerializationSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (SyntaxReceiver)context.SyntaxContextReceiver!;
            var converters = new List<string>();

            foreach (var @class in receiver.Classes)
            {
                var semanticModel = context.Compilation.GetSemanticModel(
                    @class.SyntaxTree);

                if (semanticModel.GetDeclaredSymbol(@class) is
                    not INamedTypeSymbol classSymbol)
                    throw new InvalidOperationException(
                        "Could not find named type symbol for " +
                        $"{@class.Identifier}");

                context.AddSource(
                    $"Converters.{classSymbol.Name}",
                    GenerateConverter(classSymbol));

                converters.Add($"{classSymbol.Name}Converter");
            }

            context.AddSource("SerializerOptions.Complete",
                GenerateSerializerOptionsSourceCode(converters));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(PostInitialize);
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public static void PostInitialize(
            GeneratorPostInitializationContext context)
            => context.AddSource("SerializerOptions.Template",
                GenerateSerializerOptionsTemplateSourceCode());

        internal class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<ClassDeclarationSyntax> Classes { get; } = new();

            private readonly Dictionary<string, INamedTypeSymbol> _interestingAttributes
                = new();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                _ = GetOrAddAttribute(_interestingAttributes,
                    context.SemanticModel,
                    "Discord.Net.Serialization.DiscriminatedUnionAttribute");
                _ = GetOrAddAttribute(_interestingAttributes,
                    context.SemanticModel,
                    "Discord.Net.Serialization.DiscriminatedUnionMemberAttribute");

                if (context.Node is ClassDeclarationSyntax classDecl
                    && classDecl.AttributeLists is
                        SyntaxList<AttributeListSyntax> attrList
                    && attrList.Any(
                        list => list.Attributes
                            .Any(a => IsInterestingAttribute(a,
                                context.SemanticModel,
                                _interestingAttributes.Values))))
                {
                    Classes.Add(classDecl);
                }
            }

            private static INamedTypeSymbol GetOrAddAttribute(
                Dictionary<string, INamedTypeSymbol> cache,
                SemanticModel model, string name)
            {
                if (!cache.TryGetValue(name, out var type))
                {
                    type = model.Compilation.GetTypeByMetadataName(name);
                    Debug.Assert(type != null);
                    cache.Add(name, type!);
                }

                return type!;
            }

            private static bool IsInterestingAttribute(
                AttributeSyntax attribute, SemanticModel model,
                IEnumerable<INamedTypeSymbol> interestingAttributes)
            {
                var typeInfo = model.GetTypeInfo(attribute.Name);

                return interestingAttributes.Any(
                    x => SymbolEqualityComparer.Default
                        .Equals(typeInfo.Type, x));
            }
        }
    }
}
