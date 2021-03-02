using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Regulus.Remote.Analyzer.Test
{
    
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        private const string ITestCode = @"
using System;
namespace MakeConstTest
{
    [Regulus.Remote.Syntax.Interface]
    [System.Serializable()]
    public interface ITest
    {
        Regulus.Remote.Value<bool> Get();
    }

    [Regulus.Remote.Syntax.Interface]
    public class Test
    {
    }
}";
        // </SnippetVarDeclarations>

        // <SnippetFinishedTests>
        //No diagnostics expected to show up
        [DataTestMethod]
        [DataRow(ITestCode)]        
        public void WhenTestCodeIsValidNoDiagnosticIsTriggered(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }

        
        // </SnippetFinishedTests>

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;//new MakeConstCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new RegulusRemoteAnalyzerAnalyzer(); //new MakeConstAnalyzer();
        }
    }
}
