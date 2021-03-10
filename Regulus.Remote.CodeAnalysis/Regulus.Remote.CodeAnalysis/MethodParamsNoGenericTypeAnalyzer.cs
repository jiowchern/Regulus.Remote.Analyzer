using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodParamsNoGenericTypeAnalyzer : InterfaceSyntaxNodeAnalyzer
    {        

        

        public MethodParamsNoGenericTypeAnalyzer() : base(ERRORID.RRE3, SyntaxKind.MethodDeclaration)
        {
        }





        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            var symbol = (IMethodSymbol)context.ContainingSymbol;
            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;

            foreach (var p in methodNode.ParameterList.Parameters.Zip(symbol.Parameters, (f, s) => new { Syntax = f, Symbol = s }))
            {
                var namedSymbol = p.Symbol.Type as INamedTypeSymbol;
                if (namedSymbol == null)
                    continue;

                if (!namedSymbol.IsGenericType)
                {
                    continue;
                }

                report = new Report(p.Syntax.GetLocation(), p.Symbol.Name);
                return true;
            }
            report = null;
            return false;
        }


    }
}
