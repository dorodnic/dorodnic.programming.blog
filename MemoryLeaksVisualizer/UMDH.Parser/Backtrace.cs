using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public class Backtrace
    {
        public Codebase Owner { get; private set; }
        public int TotalLeak { get; private set; }
        public int IndividualLeak { get; private set; }
        public int Count { get; private set; }

        public int StartLine { get; private set; }
        public int EndLine { get; private set; }

        public double NormalizedTotalLeak { get; set; }

        public List<LineOfCode> Lines { get; private set; }

        public static Backtrace FromLines(Codebase owner, List<string> lines, int startLine, int endLine)
        {
            var header = lines[0];
            var subHeader = lines[1];

            var headerNumbers = Regex.Matches(header, "(?<number>\\d+)( |\\))")
                .Cast<Match>()
                .Select(x => x.Groups["number"].Value)
                .Select(x => Convert.ToInt32(x))
                .ToList();

            var subHeaderNumbers = Regex.Matches(subHeader, "(?<number>\\d+)( |\\))")
                .Cast<Match>()
                .Select(x => x.Groups["number"].Value)
                .Select(x => Convert.ToInt32(x))
                .ToList();
 
            var bytesDelta = headerNumbers[0];
            var newBytes = headerNumbers[1];
            var oldBytes = headerNumbers[2];
            var countDelta = subHeaderNumbers[0];
            var newCount = subHeaderNumbers[1];
            var oldCount = subHeaderNumbers[2];

            var result = new Backtrace
            {
                TotalLeak = bytesDelta,
                IndividualLeak = bytesDelta / countDelta,
                Count = countDelta,
                Owner = owner,
                Lines = new List<LineOfCode>(),
                StartLine = startLine,
                EndLine = endLine
            };

            var justLines = lines.Skip(2).Where(x => x.Trim() != string.Empty).ToList();
            LineOfCode prevLine = null;
            foreach (var lineStr in justLines)
            {
                var line = owner.GetLine(lineStr);

                line.Leaks.AddOrGet(result); // Add leak to line's leaks
                line.Function.Leaks.AddOrGet(result); // Add leak to line's function's leak
                line.Scope.Leaks.AddOrGet(result); // Add leak to line's scope's leak
                if (line.SourceFile != null)
                {
                    line.SourceFile.Leaks.AddOrGet(result); // Add leak to line's file's leaks
                }
                line.Module.Leaks.AddOrGet(result); // Add leak to line's module's leaks

                result.Lines.Add(line); // Add line to leak

                // Connect and update calls to and called from
                if (prevLine != null)
                {
                    line.CallsTo.AddOrGet(prevLine); // Line calls prevLine
                    line.Function.CallsTo.AddOrGet(prevLine.Function); // Line's function calls prevLine's function
                    line.Module.CallsTo.AddOrGet(prevLine.Module); // Line's module calls prevLine's module

                    if (line.SourceFile != null && prevLine.SourceFile != null)
                    {
                        line.SourceFile.CallsTo.AddOrGet(prevLine.SourceFile); // Line's file calls prevLine's file
                        prevLine.SourceFile.CalledFrom.AddOrGet(line.SourceFile); // PrevLine's file is called from line's file
                    }

                    prevLine.CalledFrom.AddOrGet(line); // PrevLine is called from line
                    prevLine.Function.CalledFrom.AddOrGet(line.Function); // PrevLine's function is called from line's function
                    prevLine.Module.CalledFrom.AddOrGet(line.Module); // PrevLine's module is called from line's module
                }

                prevLine = line;
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Backtrace;
            if (other == null) return false;
            if (other.Count != Count) return false;
            if (other.IndividualLeak != IndividualLeak) return false;

            foreach (var line in Lines)
            {
                if (!other.Lines.Contains(line)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var code = IndividualLeak.GetHashCode() ^ Count.GetHashCode();

            foreach (var line in Lines)
            {
                code = code ^ line.GetHashCode();
            }

            return code;
        }

        public override string ToString()
        {
            return IndividualLeak + " bytes x " + Count + " times";
        }
    }
}
