using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Verify = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.EventSystemActionsAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;


namespace Regulus.Remote.CodeAnalysis.Test
{
    
    [TestClass]
    public class EventAnalyzerTests
    {


        [TestMethod]
        public async Task TestSystemActionsArgs()
        {
            var test = @"
namespace ConsoleApplication1
{    
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        event System.Action<IFoo> TestEvent1;
    }
}
";
            var expected = Verify.Diagnostic(ERRORID.RRE10.GetDiagnosticId()).WithSpan(7, 29, 7, 33).WithArguments("IFoo");
            await Verify.VerifyAnalyzerAsync(test, expected);
        }
        [TestMethod]
        public async Task TestSystemActions()
        {
            var test = @"
namespace ConsoleApplication1
{
    public delegate void OnTest();
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        event OnTest TestEvent1;
    }
}
";

            var expected = Verify.Diagnostic(ERRORID.RRE9.GetDiagnosticId()).WithSpan(8, 9, 8, 33).WithArguments("TestEvent1");
            await Verify.VerifyAnalyzerAsync(test, expected);
        }

    }
}
