using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public class Scope
    {
        public List<LineOfCode> Lines { get; private set; }
        public List<Function> Functions { get; private set; }
        public Codebase Owner { get; private set; }
        public string Name { get; private set; }

        public List<Scope> CallsTo { get; private set; }
        public List<Scope> CalledFrom { get; private set; }
        public List<Backtrace> Leaks { get; private set; }

        public static string ExtractScope(string symbol)
        {
            var match = Regex.Match(symbol, "(?<scope>(.+::)*)(.*)");
            var scope = match.Groups["scope"].Value;
            if (scope == string.Empty) return "(global scope)";
            return scope.TrimEnd(':');
        }

        public static Scope ParseSymbol(Codebase owner, string symbol)
        {
            return new Scope
            {
                Owner = owner,
                Name = ExtractScope(symbol),
                Functions = new List<Function>(),
                Lines = new List<LineOfCode>(),
                CalledFrom = new List<Scope>(),
                CallsTo = new List<Scope>(),
                Leaks = new List<Backtrace>()
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as Scope;
            if (other == null) return false;
            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
