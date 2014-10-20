using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public class SourceFile
    {
        public Codebase Owner { get; private set; }
        public string FullPath { get; private set; }
        public string Filename { get; private set; }
        public Module ParentModule { get; private set; }
        public List<LineOfCode> Lines { get; private set; }

        public List<SourceFile> CallsTo { get; private set; }
        public List<SourceFile> CalledFrom { get; private set; }      
        public List<Backtrace> Leaks { get; private set; }

        public static SourceFile Create(Codebase owner, Module parentModule, string path)
        {
            return new SourceFile
            {
                Owner = owner,
                FullPath = path,
                ParentModule = parentModule,
                Filename = Path.GetFileName(path),
                Lines = new List<LineOfCode>(),
                CalledFrom = new List<SourceFile>(),
                CallsTo = new List<SourceFile>(),
                Leaks = new List<Backtrace>()
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as SourceFile;
            if (other == null) return false;
            return other.FullPath == FullPath;
        }

        public override int GetHashCode()
        {
            return FullPath.GetHashCode();
        }

        public override string ToString()
        {
            return Filename;
        }
    }
}
