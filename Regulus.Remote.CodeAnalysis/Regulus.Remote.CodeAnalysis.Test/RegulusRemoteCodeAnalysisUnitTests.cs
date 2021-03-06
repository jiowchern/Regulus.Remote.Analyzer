using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.RegulusRemoteCodeAnalysisAnalyzer,
    Regulus.Remote.CodeAnalysis.RegulusRemoteCodeAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class RegulusRemoteCodeAnalysisUnitTests
    {
        [TestMethod]
        public async Task TestAttributeCheck()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
    }
}
";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
        [TestMethod]
        public async Task TestDirectReturn()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
        int Method1();
        //Regulus.Remote.Value<int> Method2();
    }
}
";
            var expected = VerifyCS.Diagnostic("RegulusRemoteCodeAnalysisReturnRule").WithSpan(7, 13, 7, 20).WithArguments("Method1");
            
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestVoidReturn()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
        void Method1();        
    }
}
";            
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestDirectReturnFix()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
        int Method1();        
        Regulus.Remote.Value<int> Method2();
    }
}
";
            var fixTest = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public interface IFoo
    {       
Regulus.Remote.Value<int>Method1();        
        Regulus.Remote.Value<int> Method2();
    }
}
";
            var expected = VerifyCS.Diagnostic("RegulusRemoteCodeAnalysisReturnRule").WithSpan(7, 13, 7, 20).WithArguments("Method1");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixTest);
        }
    }
}
