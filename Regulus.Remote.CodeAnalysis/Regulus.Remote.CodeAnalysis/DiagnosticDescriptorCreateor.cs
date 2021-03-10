using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Regulus.Remote.CodeAnalysis
{
    internal class DiagnosticDescriptorCreateor
    {
        internal readonly DiagnosticDescriptor DiagnosticDescriptor;
        

        public DiagnosticDescriptorCreateor(string diagnostic_id)
        {
            
            var title = new LocalizableResourceString($"{diagnostic_id}Title", Resources.ResourceManager, typeof(Resources));
            var message = new LocalizableResourceString($"{diagnostic_id}Message", Resources.ResourceManager, typeof(Resources));
            var description = new LocalizableResourceString($"{diagnostic_id}Description", Resources.ResourceManager, typeof(Resources));
            var dlg = new DiagnosticDescriptor(diagnostic_id, title, message, Resources.Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: description);
            DiagnosticDescriptor = dlg;
        }
    }
}