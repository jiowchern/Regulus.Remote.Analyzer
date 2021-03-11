using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyProperty = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyRemotePropertyAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;
using VerifyNotifier = Regulus.Remote.CodeAnalysis.Test.CSharpCodeFixVerifier<
    Regulus.Remote.CodeAnalysis.PropertyRemoteNotifierAnalyzer,
    Regulus.Remote.CodeAnalysis.NoFixedAnalysisCodeFixProvider>;

namespace Regulus.Remote.CodeAnalysis.Test
{
    [TestClass]
    public class PropertyPropertyReturnNoInterfaceAnalyzerTests
    {
        [TestMethod]
        public async Task TestProperty()
        {
            var test = @"
namespace ConsoleApplication1
{        
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        Regulus.Remote.Property<IFoo> Property1 { get; }
    }
}
";
            var expected = VerifyProperty.Diagnostic(ERRORID.RRE7.GetDiagnosticId()).WithSpan(7, 33, 7, 37).WithArguments("IFoo");
            await VerifyProperty.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestNotifier()
        {
            var test = @"
namespace ConsoleApplication1
{           
    [Regulus.Remote.Attributes.SyntaxHelper()]
    public interface IFoo
    {       
        Regulus.Remote.Notifier<int> Property1 { get; }
    }
}
";
            var expected = VerifyNotifier.Diagnostic(ERRORID.RRE8.GetDiagnosticId()).WithSpan(7, 33, 7, 36).WithArguments("Int32");
            await VerifyNotifier.VerifyAnalyzerAsync(test, expected);
        }
    }
}
