using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventSystemActionsAnalyzer : DiagnosticAnalyzer
    {
        
        public EventSystemActionsAnalyzer() 
        {
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ERRORID.RRE10.GetDiagnostic() , ERRORID.RRE9.GetDiagnostic());

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_Analysis, SyntaxKind.EventFieldDeclaration);
        }

        private void _Analysis(SyntaxNodeAnalysisContext context)
        {
            if (!InterfaceSyntaxNodeAnalyzer.IsInterfaceHelper(context))
                return;

            if (!_TypeCheck(context))
            {
                return;
            }
            
            var symbol = context.ContainingSymbol as IEventSymbol;
            var type = symbol.Type as INamedTypeSymbol;
            var len = type.TypeArguments.Length;
            for (int i = 0; i < len; i++)
            {
                var arg = type.TypeArguments[i] as INamedTypeSymbol;
                if (arg.TypeKind != TypeKind.Interface)
                    continue;

                var syntax = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax;
                var variableDeclarationSyntax = syntax.Declaration as Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax;
                var qualifiedNameSyntax = variableDeclarationSyntax.Type as Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax;
                var genericNameSyntax = qualifiedNameSyntax.Right as Microsoft.CodeAnalysis.CSharp.Syntax.GenericNameSyntax;
                var typeArgument = genericNameSyntax.TypeArgumentList.Arguments[i];


                var diagnostic = Diagnostic.Create(ERRORID.RRE10.GetDiagnostic(), typeArgument.GetLocation(), arg.Name);
                context.ReportDiagnostic(diagnostic);
            }


        }

        bool _TypeCheck(SyntaxNodeAnalysisContext context)
        {
            var actions = new[] {
                context.Compilation.GetTypeBySystemType(typeof(System.Action)),
                context.Compilation.GetTypeBySystemType(typeof(System.Action<>)),
                context.Compilation.GetTypeBySystemType(typeof(System.Action<,>)),
                context.Compilation.GetTypeBySystemType(typeof(System.Action<,,>)),
                context.Compilation.GetTypeBySystemType(typeof(System.Action<,,,>)),
            };
            var symbol = context.ContainingSymbol as IEventSymbol;
            if (actions.Any(a => SymbolEqualityComparer.Default.Equals(symbol.Type.OriginalDefinition, a)))
            {
                return true;
            }
            var diagnostic = Diagnostic.Create(ERRORID.RRE9.GetDiagnostic(), context.Node.GetLocation(), symbol.Name);
            context.ReportDiagnostic(diagnostic);
            return false;
        }
    }

    
}
