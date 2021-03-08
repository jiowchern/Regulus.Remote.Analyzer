using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodParamsCountAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "rr0004";
        
        static readonly DiagnosticDescriptor _DiagnosticDescriptor= new DiagnosticDescriptorCreateor(DiagnosticId).DiagnosticDescriptor;
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_DiagnosticDescriptor);
        public MethodParamsCountAnalyzer()
        {
        
        }
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_MethodAnalysis, Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration);
        }

        private void _MethodAnalysis(SyntaxNodeAnalysisContext context)
        {
            var symbol = (IMethodSymbol)context.ContainingSymbol;
            var attrs = symbol.ContainingSymbol.GetAttributes();
            var checkerType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            if (!attrs.ContainsAttributeType(checkerType))
                return;

            if (symbol.ReceiverType.TypeKind != TypeKind.Interface)
                return;

            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;
            var lastParam = methodNode.ParameterList.Parameters.Zip(symbol.Parameters, (f, s) => new { Syntax = f, Symbol = s }).Skip(5).LastOrDefault();
            if (lastParam == null)
                return;

            var diagnostic = Diagnostic.Create(_DiagnosticDescriptor, lastParam.Syntax.GetLocation(), lastParam.Symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
