using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodParamsCountAnalyzer : InterfaceSyntaxNodeAnalyzer
    {
        

        public MethodParamsCountAnalyzer() : base(ERRORID.RRE4, SyntaxKind.MethodDeclaration)
        {
        }

        

        

        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            report = null;
            var symbol = (IMethodSymbol)context.ContainingSymbol;
            var attrs = symbol.ContainingSymbol.GetAttributes();
            var checkerType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            if (!attrs.ContainsAttributeType(checkerType))
                return false;

            if (symbol.ReceiverType.TypeKind != TypeKind.Interface)
                return false;

            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;
            var lastParam = methodNode.ParameterList.Parameters.Zip(symbol.Parameters, (f, s) => new { Syntax = f, Symbol = s }).Skip(5).LastOrDefault();
            if (lastParam == null)
                return false;
            report = new Report(lastParam.Syntax.GetLocation(), lastParam.Symbol.Name);
            return true;
        }
    }
}
