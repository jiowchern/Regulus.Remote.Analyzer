using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RegulusRemoteCodeAnalysisAnalyzer : DiagnosticAnalyzer
    {
        
        public const string DiagnosticId = "RegulusRemoteCodeAnalysisReturnRule";
        private static readonly LocalizableString TitleReturnRule = new LocalizableResourceString(nameof(Resources.TitleReturnRule), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageReturnRule = new LocalizableResourceString(nameof(Resources.MessageReturnRule), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString DescriptionReturnRule = new LocalizableResourceString(nameof(Resources.DescriptionReturnRule), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor ReturnRule = new DiagnosticDescriptor(DiagnosticId, TitleReturnRule, MessageReturnRule, Resources.CategoryReturnRule, DiagnosticSeverity.Error, isEnabledByDefault: true, description: DescriptionReturnRule);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(ReturnRule); } }
        public RegulusRemoteCodeAnalysisAnalyzer()
        {

        }
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            context.RegisterSymbolAction(_MethodTypeCheck, SymbolKind.Method);
        }
        private void _MethodTypeCheck(SymbolAnalysisContext context)
        {
            var symbol = (IMethodSymbol)context.Symbol;
            var attrs = symbol.ReceiverType.GetAttributes();
            var checkerType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Attributes.SyntaxCheck");
            if (!attrs.ContainsAttributeType(checkerType))
                return;
            var retType = symbol.ReturnType;
            var returnType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, returnType))
            {
                return;
            }

            var voidType = context.Compilation.GetTypeByMetadataName("System.Void");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, voidType))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(ReturnRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
