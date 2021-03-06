using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FixableReturnTypeAnalyzer : DiagnosticAnalyzer
    {
        
        public const string DiagnosticId = "rr0001";
        private static readonly LocalizableString TitleReturnRule = new LocalizableResourceString(nameof(Resources.TitleReturnRule), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageReturnRule = new LocalizableResourceString(nameof(Resources.MessageReturnRule), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString DescriptionReturnRule = new LocalizableResourceString(nameof(Resources.DescriptionReturnRule), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor ReturnRule = new DiagnosticDescriptor(DiagnosticId, TitleReturnRule, MessageReturnRule, Resources.Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: DescriptionReturnRule);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(ReturnRule); } }
        public FixableReturnTypeAnalyzer()
        {

        }
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_MethodTypeCheck , Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration);
        }

        private void _MethodTypeCheck(SyntaxNodeAnalysisContext context)
        {
            var symbol = (IMethodSymbol)context.ContainingSymbol;
            var attrs = symbol.ReceiverType.GetAttributes();
            var checkerType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Attributes.SyntaxCheck");
            if (!attrs.ContainsAttributeType(checkerType))
                return;

            if (symbol.ReceiverType.TypeKind != TypeKind.Interface)
                return;

            var retType = symbol.ReturnType as INamedTypeSymbol;

            if (retType.IsGenericType)
                return;

            var valueType = context.Compilation.GetTypeByMetadataName("Regulus.Remote.Value`1");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, valueType))
            {
                return;
            }

            var voidType = context.Compilation.GetTypeByMetadataName("System.Void");
            if (SymbolEqualityComparer.Default.Equals(retType.OriginalDefinition, voidType))
            {
                return;
            }

            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;            
            var diagnostic = Diagnostic.Create(ReturnRule, methodNode.ReturnType.GetLocation(), retType.Name);
            context.ReportDiagnostic(diagnostic);
        }

      
    }
}
