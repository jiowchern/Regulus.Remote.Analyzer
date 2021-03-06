using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace Regulus.Remote.CodeAnalysis
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NoFixedAnalysisCodeFixProvider)), Shared]
    public class NoFixedAnalysisCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray<string>.Empty;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            
        }
    }
}
