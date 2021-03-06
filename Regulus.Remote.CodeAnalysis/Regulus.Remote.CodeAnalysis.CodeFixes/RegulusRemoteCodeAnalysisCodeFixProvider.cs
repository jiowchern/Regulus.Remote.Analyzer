using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace Regulus.Remote.CodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RegulusRemoteCodeAnalysisCodeFixProvider)), Shared]
    public class RegulusRemoteCodeAnalysisCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RegulusRemoteCodeAnalysisAnalyzer.DiagnosticId); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            
        }
    }
}
