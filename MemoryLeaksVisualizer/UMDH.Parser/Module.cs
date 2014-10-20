using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public class Module
    {
        public Codebase Owner { get; private set; }
        public string Name { get; private set; }
        public string SymbolsFile { get; private set; }
        public List<Scope> Scopes { get; private set; }
        public List<SourceFile> Files { get; private set; }
        public List<LineOfCode> Lines { get; private set; }
        public List<Function> Functions { get; private set; }

        public List<Module> CallsTo { get; private set; }
        public List<Module> CalledFrom { get; private set; }
        public List<Backtrace> Leaks { get; private set; }

        public static Module Create(Codebase owner, string name, string symbolsFile)
        {
            return new Module
            {
                Name = name,
                Owner = owner,
                SymbolsFile = symbolsFile,
                Scopes = new List<Scope>(),
                Files = new List<SourceFile>(),
                Lines = new List<LineOfCode>(),
                Functions = new List<Function>(),
                CalledFrom = new List<Module>(),
                CallsTo = new List<Module>(),
                Leaks = new List<Backtrace>()
            };
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Module;
            if (other == null) return false;
            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
