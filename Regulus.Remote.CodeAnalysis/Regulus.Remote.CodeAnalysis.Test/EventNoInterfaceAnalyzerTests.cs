using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Verify = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.EventNoInterfaceAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;
namespace Regulus.Remote.CodeAnalysis.Test
{
    
    [TestClass]
    public class EventNoInterfaceAnalyzerTests
    {
        [TestMethod]
        public async Task Test()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        event System.Action<IFoo> Event1 ;
    }
}
";

            var expected = Verify.Diagnostic(ERRORID.RRE9.GetDiagnosticId()).WithSpan(7, 9, 7, 12).WithArguments("Int32");
            await Verify.VerifyAnalyzerAsync(test, expected);
        }

    }
}
