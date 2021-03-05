using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RegulusRemoteCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RegulusRemoteCodeAnalysis";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor UpperRule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static readonly DiagnosticDescriptor ReturnRule = new DiagnosticDescriptor("RegulusRemoteCodeAnalysisReturnRule", Resources.TitleReturnRule, Resources.MessageReturnRule, Resources.CategoryReturnRule , DiagnosticSeverity.Error, isEnabledByDefault: true, description: Resources.DescriptionReturnRule);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(UpperRule, ReturnRule); } }
        public RegulusRemoteCodeAnalysisAnalyzer()
        {

        }
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSymbolAction(_NamedTypeCheck, SymbolKind.NamedType);
            context.RegisterSymbolAction(_MethodTypeCheck, SymbolKind.Method);
        }

        private void _MethodTypeCheck(SymbolAnalysisContext context)
        {
            var symbol = (IMethodSymbol)context.Symbol;
            var attrs = symbol.ReceiverType.GetAttributes();
            var checkerType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Attributes.SyntaxCheck");
            if (!attrs.ContainsAttributeType(checkerType))
                return;

            var returnType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");


            var retType = symbol.ReturnType;
            
            if (!SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, returnType))
            {
                var diagnostic = Diagnostic.Create(ReturnRule, symbol.Locations[0], symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void _NamedTypeCheck(SymbolAnalysisContext context)
        {            
            var checker = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Attributes.SyntaxCheck"); 
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
            var attrs = namedTypeSymbol.GetAttributes();            
            if (!attrs.ContainsAttributeType(checker))
                return;

            var methods = namedTypeSymbol.MemberNames.ToArray();


        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(UpperRule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
