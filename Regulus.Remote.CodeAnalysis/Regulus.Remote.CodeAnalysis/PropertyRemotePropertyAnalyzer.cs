﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyRemotePropertyAnalyzer : InterfaceSyntaxNodeAnalyzer
    {
        public PropertyRemotePropertyAnalyzer() : base(ERRORID.RRE7, SyntaxKind.PropertyDeclaration)
        {

        }

       

        

        public override bool NeedReport(SyntaxNodeAnalysisContext context, out Report report)
        {
            report = null;
            var symbol = context.ContainingSymbol as IPropertySymbol;

            var propertySyntax = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Property<>));
            var namedType = symbol.Type as INamedTypeSymbol;
            if (!SymbolEqualityComparer.Default.Equals(namedType.ConstructedFrom, propertySyntax))
            {
                return false;
            }
            var type = symbol.Type as INamedTypeSymbol;
            var typeArg = type.TypeArguments[0];
            if (typeArg.TypeKind != TypeKind.Interface)
            {
                return false;
            }
            var syntax = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax;

            var typeSyntax = syntax.Type as Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax;
            var genericTypeSyntax = typeSyntax.Right as Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax;
            var node = genericTypeSyntax.TypeArgumentList.Arguments[0];

            report = new Report(node.GetLocation(), typeArg.Name);
            return true;
        }
    }
}
