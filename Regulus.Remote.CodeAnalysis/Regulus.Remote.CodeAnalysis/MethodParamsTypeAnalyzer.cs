using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodParamsTypeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticInterfaceId = "rr0002";
        private static readonly LocalizableString Title0002 = new LocalizableResourceString(nameof(Resources.TitleParamNotInterface), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Message0002 = new LocalizableResourceString(nameof(Resources.MessageParamNotInterface), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description0002= new LocalizableResourceString(nameof(Resources.DescriptionParamNotInterface), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor Rule0002 = new DiagnosticDescriptor(DiagnosticInterfaceId, Title0002, Message0002, Resources.Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description0002);

        public const string DiagnosticGenericId = "rr0003";
        private static readonly LocalizableString Title0003 = new LocalizableResourceString(nameof(Resources.TitleParamNotGeneric), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Message0003 = new LocalizableResourceString(nameof(Resources.MessageParamNotGeneric), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description0003 = new LocalizableResourceString(nameof(Resources.DescriptionParamNotGeneric), Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor Rule0003 = new DiagnosticDescriptor(DiagnosticGenericId, Title0003, Message0003, Resources.Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description0003);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule0002, Rule0003); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_MethodTypeCheck, Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration);
        }

        private void _MethodTypeCheck(SyntaxNodeAnalysisContext context)
        {
            var symbol = (IMethodSymbol)context.ContainingSymbol;
            var attrs = symbol.ContainingSymbol.GetAttributes();
                        
            var checkerType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            if (!attrs.ContainsAttributeType(checkerType))
                return;

            if (symbol.ReceiverType.TypeKind != TypeKind.Interface)
                return;

            var methodNode = context.Node as Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax;

            
            foreach (var p in methodNode.ParameterList.Parameters.Zip(symbol.Parameters, (f, s) => new { Syntax = f, Symbol = s }))
            {
                if(p.Symbol.Type.TypeKind != TypeKind.Interface)
                {
                    continue;
                }
                
                var diagnostic = Diagnostic.Create(Rule0002, p.Syntax.GetLocation(), p.Symbol.Name);
                context.ReportDiagnostic(diagnostic);
                return;
            }

            foreach (var p in methodNode.ParameterList.Parameters.Zip(symbol.Parameters, (f, s) => new { Syntax = f, Symbol = s }))
            {
                var namedSymbol = p.Symbol.Type as INamedTypeSymbol;
                if (namedSymbol == null)
                    continue;

                if (!namedSymbol.IsGenericType)
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(Rule0003, p.Syntax.GetLocation(), p.Symbol.Name);
                context.ReportDiagnostic(diagnostic);
                return;
            }

        }
    }
}
