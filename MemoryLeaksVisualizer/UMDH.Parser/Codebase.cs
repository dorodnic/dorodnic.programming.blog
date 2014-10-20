using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    /// <summary>
    /// Repersents all information extracted from UMDH delta file
    /// </summary>
    public class Codebase
    {
        public List<Module> Modules { get; private set; }
        public List<LineOfCode> Lines { get; private set; }
        public List<SourceFile> Files { get; private set; }
        public List<Function> Functions { get; private set; }
        public List<Scope> Scopes { get; private set; }
        public List<Backtrace> Leaks { get; private set; }

        public long TotalLeak { get { return Leaks.Sum(x => x.TotalLeak); } } 
        public int ModulesCount { get { return Modules.Count(); } }
        public int LeaksCount { get { return Leaks.Count(); } }

        public object Owner { get; set; }

        // Copy of original UMDH report
        public string RawData { get; set; }

        // Max total leak
        public long MaxLeak { get; private set; }
        
        public Codebase()
        {
            Lines = new List<LineOfCode>();
            Modules = new List<Module>();
            Functions = new List<Function>();
            Files = new List<SourceFile>();
            Scopes = new List<Scope>();
            Leaks = new List<Backtrace>();
        }

        // Prepare codebase for display
        public void Normalize()
        {
            Leaks = Leaks.OrderByDescending(x => x.TotalLeak).ToList();

            MaxLeak = Leaks.Any() ? Leaks.Max(x => x.TotalLeak) : 1;
            foreach (var leak in Leaks)
            {
                leak.NormalizedTotalLeak = (double)leak.TotalLeak / MaxLeak;
            }

            Lines = Lines.OrderBy(x => x.Function.Name).ToList();
        }

        // Get line if it already exists inside the codebase,
        // or add new if not found
        public LineOfCode GetLine(string line)
        {
            line = line.Trim();

            var existingLine = Lines.FirstOrDefault(x => x.ID == line);
            if (existingLine != null)
            {
                return existingLine;
            }
            else
            {
                var newLine = LineOfCode.Parse(this, line);
                Lines.Add(newLine);
                return newLine;
            }
        }

        // Add new backtrace
        public void AddBacktrace(List<string> lines, int startLine, int endLine)
        {
            var newTrace = Backtrace.FromLines(this, lines, startLine, endLine);
            Leaks.Add(newTrace);
        }

        // Get existing function or add new one from symbol
        public Function GetFunction(string symbol)
        {
            var existing = Functions.FirstOrDefault(x => x.SymbolName == symbol);
            if (existing != null)
            {
                return existing;
            }
            else
            {
                var newFunction = Function.ParseSymbol(this, symbol);
                Functions.Add(newFunction);
                return newFunction;
            }
        }

        // Get existing scope or add new one from symbol
        public Scope GetScope(string symbol)
        {
            var existing = Scopes.FirstOrDefault(x => x.Name 
                == Scope.ExtractScope(symbol));
            if (existing != null)
            {
                return existing;
            }
            else
            {
                var newScope = Scope.ParseSymbol(this, symbol);
                Scopes.Add(newScope);
                return newScope;
            }
        }

        // Get existing file or add new one
        public SourceFile GetFile(string path, Module module)
        {
            var existing = Files.FirstOrDefault(x => x.FullPath == path);
            if (existing != null)
            {
                return existing;
            }
            else
            {
                var newFile = SourceFile.Create(this, module, path);
                Files.Add(newFile);
                return newFile;
            }
        }
    }
}
