using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Analyzers
{
}

namespace Regulus.Remote.Analyzer.Test
{

    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        private const string ITestCode = @"
using System;
namespace MakeConstTest
{
    [Regulus.Remote.Syntax.CheckInterfaceAttribute]    
    [System.Serializable]
    public interface ITest
    {
        Regulus.Remote.Value<bool> Get();
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
        [DataTestMethod]
        public async void Test1()
        {
            var project =  Analyzers.can_compile_and_analyze.MakeProjectWith(ITestCode);
            var dils = await Analyzers.can_compile_and_analyze.CompileAndAnalyze(project, new RegulusRemoteAnalyzerAnalyzer());            
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
