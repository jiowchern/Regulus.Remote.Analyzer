using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;



namespace Regulus.Remote.Analyzer
{
    internal static class SymbolExtensions
    {
        internal static bool ContainsAttributeType(this ImmutableArray<AttributeData> attributes, INamedTypeSymbol attributeType, bool exactMatch = false)
            => attributes.Any(a => attributeType.IsAssignableFrom(a.AttributeClass, exactMatch));

        internal static bool IsAssignableFrom(this INamedTypeSymbol targetType, INamedTypeSymbol sourceType, bool exactMatch = false)
        {
            
            if (targetType != null)
            {
                while (sourceType != null)
                {
                    
                    if (Equals(sourceType, targetType))
                        return true;

                    if (exactMatch)
                        return false;

                    if (targetType.TypeKind == TypeKind.Interface)
                        return sourceType.AllInterfaces.Any(i => i.Equals(targetType));

                    sourceType = sourceType.BaseType;
                }
            }

            return false;
        }
    }
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RegulusRemoteAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RegulusRemoteAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        private INamedTypeSymbol _NameType;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterCompilationStartAction(c => {
                _NameType = c.Compilation.GetTypeByMetadataName("Regulus.Remote.Syntax.CheckInterfaceAttribute");
                context.RegisterSymbolAction(_MethodAnalyze, SymbolKind.Method);
                context.RegisterSymbolAction(_NameAnalyze, SymbolKind.NamedType);
            });
            
        }

        private void _NameAnalyze(SymbolAnalysisContext context)
        {            
            /*var valueType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
            var notifierType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Notifier`1");
            var propertyType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Property`1");
            var voidType = context.Compilation.GetTypeByMetadataName("System.Void");*/
            var symbol = (INamedTypeSymbol)context.Symbol;
            var attrs = symbol.GetAttributes();
            if (!attrs.ContainsAttributeType(_NameType))
                return;
            /*if (_ReturnTypeCheck(symbol))
            {
                return;
            }*/
        }

        private void _MethodAnalyze(SymbolAnalysisContext context)
        {            
            
            /*var valueType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
            var notifierType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Notifier`1");
            var propertyType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Property`1");
            var voidType = context.Compilation.GetTypeByMetadataName("System.Void");*/
            var symbol = (IMethodSymbol)context.Symbol;            
            var attrs = symbol.ReceiverType.GetAttributes();
            if (!attrs.ContainsAttributeType(_NameType))
                return;
            if(_ReturnTypeCheck(symbol))
            {
                return;
            }
            
        }

        private bool _ReturnTypeCheck(IMethodSymbol symbol)
        {
            var retType = symbol.ReturnType;
            return true;
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
