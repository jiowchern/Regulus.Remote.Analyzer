using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.FixableReturnTypeAnalyzer,
    Regulus.Remote.CodeAnalysis.FixableReturnTypeAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class RegulusRemoteCodeAnalysisUnitTests
    {
    

        [TestMethod]
        public async Task TestReturnTypeClassIgnore()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public class IFoo
    {       
        int Method()
        {
            return 0;
        }
    }
}
";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestReturnTypeStaticClassIgnore()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxCheck()]
    public static class Foo
    {       
        static int Method(this int val )
        {
            return 0;
        }
    }
}
";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
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
            var expected = VerifyCS.Diagnostic(FixableReturnTypeAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("Int32");
            
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
Regulus.Remote.Value<int> Method1();        
        Regulus.Remote.Value<int> Method2();
    }
}
";
            var expected = VerifyCS.Diagnostic(FixableReturnTypeAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("Int32");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixTest);
        }
    }
}
