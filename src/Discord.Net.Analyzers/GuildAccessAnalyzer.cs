using Discord.Commands;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class GuildAccessAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "DNET0001";
        private const string Title = "Limit command to Guild contexts.";
        private const string MessageFormat = "Command method '{0}' is accessing 'Context.Guild' but is not restricted to Guild contexts.";
        private const string Description = "Accessing 'Context.Guild' in a command without limiting the command to run only in guilds.";
        private const string Category = "API Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            // Bail out if the accessed member isn't named 'Guild'
            var memberAccessSymbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;
            if (memberAccessSymbol.Name != "Guild")
                return;

            // Bail out if it happens to be 'ContextType.Guild' in the '[RequireContext]' argument
            if (context.Node.Parent is AttributeArgumentSyntax)
                return;

            // Bail out if the containing class doesn't derive from 'ModuleBase<T>'
            var typeNode = context.Node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeNode);
            if (!typeSymbol.DerivesFromModuleBase())
                return;

            // Bail out if the containing method isn't marked with '[Command]'
            var methodNode = context.Node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
            var methodAttributes = methodSymbol.GetAttributes();
            if (!methodAttributes.Any(a => a.AttributeClass.Name == nameof(CommandAttribute)))
                return;

            // Is the '[RequireContext]' attribute not applied to either the
            // method or the class, or its argument isn't 'ContextType.Guild'?
            var ctxAttribute = methodAttributes.SingleOrDefault(_attributeDataPredicate)
                ?? typeSymbol.GetAttributes().SingleOrDefault(_attributeDataPredicate);

            if (ctxAttribute == null || ctxAttribute.ConstructorArguments.Any(arg => !arg.Value.Equals((int)ContextType.Guild)))
            {
                // Report the diagnostic
                var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static readonly Func<AttributeData, bool> _attributeDataPredicate =
            (a => a.AttributeClass.Name == nameof(RequireContextAttribute));
    }
}
