using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public class LineOfCode
    {
        public string ID { get; private set; }
        public Codebase Owner { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public int LineNumber { get; private set; }
        public Module Module { get; private set; }
        public Function Function { get; private set; }
        public Scope Scope { get; private set; }

        public string Preview { get; private set; }

        public List<LineOfCode> CalledFrom { get; private set; }
        public List<LineOfCode> CallsTo { get; private set; }
        public List<Backtrace> Leaks { get; private set; }


        private LineOfCode() { }

        public static LineOfCode Parse(Codebase owner, string line)
        {
            line = line.Trim();
            var result = new LineOfCode();
            result.Owner = owner;
            result.ID = line;

            result.CalledFrom = new List<LineOfCode>();
            result.CallsTo = new List<LineOfCode>();
            result.Leaks = new List<Backtrace>();

            var match = Regex.Match(line, "(?<moduleName>.*)\\!(?<symbolName>.*)\\+(?<rest>(.*))?");
            var moduleName = match.Groups["moduleName"].Value;
            var symbolName = match.Groups["symbolName"].Value;
            var rest = match.Groups["rest"].Success ? match.Groups["rest"].Value : "";

            result.Module = owner.Modules.FirstOrDefault(x => x.Name == moduleName);
            result.Scope = owner.GetScope(symbolName);
            result.Function = owner.GetFunction(symbolName);

            result.Scope.Lines.Add(result); // Add line to scope
            result.Function.Lines.AddOrGet(result); // Add line to function

            result.Module.Scopes.AddOrGet(result.Scope); // Add scope to module
            result.Module.Functions.AddOrGet(result.Function); // Add function to module
            result.Module.Lines.AddOrGet(result); // Add line to module

            match = Regex.Match(rest, ".* \\((?<fileName>.*), (?<lineNum>\\d*)\\)");
            if (match.Success) // Is file,line present?
            {
                var filename = match.Groups["fileName"].Value;
                var lineNum = Convert.ToInt32(match.Groups["lineNum"].Value);

                result.LineNumber = lineNum;
                result.SourceFile = owner.GetFile(filename, result.Module);
                result.SourceFile.Lines.AddOrGet(result); // Add line to file

                result.Module.Files.AddOrGet(result.SourceFile); // Add file to module

                result.GeneratePreview(filename, lineNum);
            }

            return result;
        }

        private void GeneratePreview(string filename, int lineNum)
        {
            if (File.Exists(filename))
            {
                var lines = File.ReadAllLines(filename);
                if (lines.Count() >= lineNum && lineNum > 0)
                {
                    Preview = lines[lineNum - 1].Trim();
                    if (lineNum > 1) Preview = lines[lineNum - 2].Trim() + "\r\n" + Preview;
                    if (lines.Count() > lineNum) Preview = Preview + "\r\n" + lines[lineNum].Trim();
                }
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as LineOfCode;
            if (other == null) return false;
            return other.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public string ShortID
        {
            get
            {
                if (SourceFile != null)
                {
                    return Function.Name + " (" + SourceFile.Filename + ", " + LineNumber + ")";
                }
                else
                {
                    return ID;
                }
            }
        }

        public override string ToString()
        {
            return ID;
        }
    }
}
