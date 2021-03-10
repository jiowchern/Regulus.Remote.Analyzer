using Microsoft.CodeAnalysis;

namespace Regulus.Remote.CodeAnalysis
{
    public class DiagnosticDescriptorProvider  
    {
        
        readonly System.Collections.Generic.Dictionary<ERRORID, DiagnosticDescriptor> _DiagnosticDescriptors;
        public readonly System.Collections.Generic.IReadOnlyDictionary<ERRORID, DiagnosticDescriptor> Descriptors;

        public DiagnosticDescriptor this[ERRORID id]
        {
            get {
                return Descriptors[id];
            }
        }

        public static readonly DiagnosticDescriptorProvider Instance = Regulus.Utility.Singleton<DiagnosticDescriptorProvider>.Instance;
        public DiagnosticDescriptorProvider() 
        {

            _DiagnosticDescriptors = new System.Collections.Generic.Dictionary<ERRORID, DiagnosticDescriptor>();

            foreach (var item in Regulus.Utility.EnumHelper.GetEnums<ERRORID>())
            {
                _DiagnosticDescriptors.Add(item , new DiagnosticDescriptorCreateor(item.ToString()).DiagnosticDescriptor);
            }

            Descriptors = _DiagnosticDescriptors;
        }

        
    }
}
