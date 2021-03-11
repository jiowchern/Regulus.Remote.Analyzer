namespace Regulus.Remote.CodeAnalysis
{
    public enum ERRORID
    {
        RRE1 = 1,
        RRE2,
        RRE3,
        RRE4,
        RRE5,
        RRE6,
        RRE7,
        RRE8,
    }

    public static class ERRORIDExtensions
    {
        public static Microsoft.CodeAnalysis.DiagnosticDescriptor GetDiagnostic(this ERRORID id)
        {
            return DiagnosticDescriptorProvider.Instance[id];
        }
        public static string GetDiagnosticId(this ERRORID id)
        {
            return DiagnosticDescriptorProvider.Instance[id].Id;
        }
    }
}
