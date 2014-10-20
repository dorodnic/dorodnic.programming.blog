using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public class Function
    {
        public Codebase Owner { get; private set; }
        public string SymbolName { get; private set; }
        public Scope DeclaringScope { get; private set; }
        public string Name { get; private set; }
        public List<LineOfCode> Lines { get; private set; }

        public List<Function> CallsTo { get; private set; }
        public List<Function> CalledFrom { get; private set; }
        public List<Backtrace> Leaks { get; private set; }

        public static Function ParseSymbol(Codebase owner, string symbol)
        {
            var match = Regex.Match(symbol, "((.+::)*)(?<functionName>.*)");
            var name = match.Groups["functionName"].Value;

            var scope = owner.GetScope(symbol);

            var result = new Function
            {
                Owner = owner,
                SymbolName = symbol,
                Name = name,
                DeclaringScope = scope,
                Lines = new List<LineOfCode>(),
                CalledFrom = new List<Function>(),
                CallsTo = new List<Function>(),
                Leaks = new List<Backtrace>()
            };

            scope.Functions.AddOrGet(result);

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Function;
            if (other == null) return false;
            return other.SymbolName == SymbolName;
        }

        public override int GetHashCode()
        {
            return SymbolName.GetHashCode();
        }

        public override string ToString()
        {
            return SymbolName;
        }
    }
}
