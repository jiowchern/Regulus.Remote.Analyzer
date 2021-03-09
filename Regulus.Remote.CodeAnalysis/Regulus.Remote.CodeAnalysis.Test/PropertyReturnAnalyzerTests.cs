using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using VerifyNoFixedCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyReturnAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

using VerifyFixToPropertyCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyReturnAnalyzer,
    Regulus.Remote.CodeAnalysis.PropertyReturnPropertyAnalyzerCodeFixProvider>;

using VerifyFixToNotifierCS = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyReturnAnalyzer,
    Regulus.Remote.CodeAnalysis.PropertyReturnNotifierAnalyzerCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{

    [TestClass]
    public class PropertyReturnAnalyzerTests
    {
        [TestMethod]
        public async Task FixedReturnToPropertyTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        int Property1 { get; }
    }
}
";
            var fixTest = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
Regulus.Remote.Property<int> Property1 { get; }
    }
}
";
            var expected = VerifyFixToPropertyCS.Diagnostic(PropertyReturnAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("Int32");
            await VerifyFixToPropertyCS.VerifyCodeFixAsync(test, expected, fixTest);
        }

        [TestMethod]
        public async Task FixedReturnToNotifierTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        int Property1 { get; }
    }
}
";
            var fixTest = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
Regulus.Remote.Notifier<int> Property1 { get; }
    }
}
";
            var expected = VerifyFixToNotifierCS.Diagnostic(PropertyReturnAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("Int32");
            await VerifyFixToNotifierCS.VerifyCodeFixAsync(test, expected, fixTest);
        }


        [TestMethod]
        public async Task NoFixTest()
        {
            var test = @"
namespace ConsoleApplication1
{
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        int Property1 { get; }
    }
}
";
  
            var expected = VerifyNoFixedCS.Diagnostic(PropertyReturnAnalyzer.DiagnosticId).WithSpan(7, 9, 7, 12).WithArguments("int");
            await VerifyNoFixedCS.VerifyAnalyzerAsync(test, expected);            
        }

    }
}
