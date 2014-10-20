using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace UMDH.Visualizer
{
    public static class VisualStudioHelper
    {
        public static string GetVisualStudioObjectName(string version = null)
        {
            const string template = "VisualStudio.DTE.{0}.0";

            switch (version)
            {
                case "2005": return string.Format(template, 8);
                case "2008": return string.Format(template, 9);
                case "2010": return string.Format(template, 10);
                case "2012": return string.Format(template, 11);
                default: return "VisualStudio.DTE"; // System Default
            }
        }

        public static string DetectVersion(List<string> versions)
        {
            foreach (var version in versions)
            {
                var vsObjectName = GetVisualStudioObjectName(version);

                try
                {
                    // Try to connect to an instance that is already open
                    System.Runtime.InteropServices.Marshal.GetActiveObject(vsObjectName);
                    return version;
                }
                catch
                {
                    
                }
            }

            return versions.First();
        }

        public static void OpenInVisualStudio(string vsObjectName, string file, int line)
        {
            DTE dte;
            try
            {
                // Try to connect to an instance that is already open
                dte = System.Runtime.InteropServices.Marshal.GetActiveObject(vsObjectName) as DTE;
            }
            catch
            {
                // Create DTE in a new instance
                var t = Type.GetTypeFromProgID(vsObjectName);
                dte = Activator.CreateInstance(t) as DTE; ;
            }

            if (dte != null)
            {
                dte.MainWindow.Visible = true;
                dte.UserControl = true;
                var window = dte.ItemOperations.OpenFile(file);
                var selection = dte.ActiveDocument.Selection as TextSelection;
                selection.GotoLine(line, true); // Goto and select line
            }
        }
    }
}
