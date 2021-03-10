using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyReturnAnalyzer : InterfaceSyntaxNodeAnalyzer
    {


        public PropertyReturnAnalyzer() : base(ERRORID.RRE5, SyntaxKind.PropertyDeclaration)
        {
        }
        

        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            report = null;
            var symbol = context.ContainingSymbol as IPropertySymbol;
            var syntax = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax;

            var attrs = symbol.ContainingSymbol.GetAttributes();
            var checkerType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            if (!attrs.ContainsAttributeType(checkerType))
                return false;

            if (symbol.ContainingType.TypeKind != TypeKind.Interface)
                return false; 

            var retSymbol = symbol.Type as INamedTypeSymbol;
            var retSyntax = syntax.Type;

            var propertySymbol = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Property<>));
            if (SymbolEqualityComparer.Default.Equals(retSymbol.OriginalDefinition, propertySymbol))
            {
                return false;
            }

            var notifierSymbol = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Notifier<>));
            if (SymbolEqualityComparer.Default.Equals(retSymbol.OriginalDefinition, notifierSymbol))
            {
                return false;
            }
            report = new Report(retSyntax.GetLocation(), retSymbol.Name);
            return true;            
        }

   
    }
}
