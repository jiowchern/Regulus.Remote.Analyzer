using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class InterfaceSyntaxNodeAnalyzer : DiagnosticAnalyzer
    {

        public class Report
        {
            public readonly object[] Args;
            public readonly Location Location;            

            public Report(Location location, params object[]  args)
            {
                Location = location;
                Args = args;
            }
        }
        public readonly DiagnosticDescriptor Descriptor;
        private readonly SyntaxKind[] _Kinds;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public InterfaceSyntaxNodeAnalyzer(ERRORID id , params SyntaxKind[] kinds)
        {
            Descriptor = id.GetDiagnostic();
            this._Kinds = kinds;
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_Analysis, _Kinds);
        }
        public abstract bool NeedReport(SyntaxNodeAnalysisContext context,out Report report);
        private void _Analysis(SyntaxNodeAnalysisContext context)
        {
            if(_NoHelp(context))            
                return;
            
            if (_NoInterface(context))
                return;

            Report report;
            if(NeedReport(context, out report))
            {
                var diagnostic = Diagnostic.Create(Descriptor, report.Location , report.Args);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool _NoInterface(SyntaxNodeAnalysisContext context)
        {
            return context.ContainingSymbol.ContainingType.TypeKind != TypeKind.Interface;                
        }
        public static bool IsInterfaceHelper(SyntaxNodeAnalysisContext context)
        {
            if (_NoHelp(context))
                return false;

            if (_NoInterface(context))
                return false;

            return true;
        }
        private static  bool _NoHelp(SyntaxNodeAnalysisContext context)
        {
            var parentSymbol = context.ContainingSymbol.ContainingType;
            var helperType = context.Compilation.GetTypeBySystemType(typeof(Regulus.Remote.Attributes.SyntaxHelper));
            return !parentSymbol.GetAttributes().ContainsAttributeType(helperType);
        }
    }
}
