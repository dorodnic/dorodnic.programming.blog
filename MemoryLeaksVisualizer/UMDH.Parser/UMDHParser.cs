using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public static class UMDHParser
    {
        public static Codebase Parse(string path, Action<int> onProgress = null)
        {
            if (!File.Exists(path)) throw new Exception("File \"" + path + "\" not found!");

            var lines = File.ReadAllLines(path);

            if (onProgress != null) onProgress(10);

            var blocksStarts = new List<int>();

            // Detect block edges
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                // Foreach block start
                foreach (Match match in Regex.Matches(line, "\\+( )*\\d+ \\(( )*\\d+ \\-( )*\\d"))
                {
                    var start = match.Index;

                    if (blocksStarts.Any())
                    {
                        if (blocksStarts.Last() < i - 1)
                        {
                            blocksStarts.Add(i);
                        }
                    }
                    else
                    {
                        blocksStarts.Add(i);
                    }
                }
            }
            blocksStarts.Add(lines.Count() - 1);

            if (onProgress != null) onProgress(20);

            var codebase = new Codebase();

            codebase.RawData = File.ReadAllText(path);

            if (onProgress != null) onProgress(30);

            // Detect and parse modules
            for (int i = 0; i < blocksStarts[0]; i++)
            {
                var line = lines[i];
                var match = Regex.Match(line, "DBGHELP\\: (?<moduleName>.*) - (?<label>.*)");
                if (match.Success)
                {
                    var symbolsFile = string.Empty;
                    if ("private symbols & lines " == match.Groups["label"].Value)
                    {
                        symbolsFile = lines[i + 1].Trim();
                    }
                    codebase.Modules.Add(Module.Create(codebase, match.Groups["moduleName"].Value, symbolsFile));
                }
            }

            if (onProgress != null) onProgress(50);

            // Parse the backtraces
            for (int i = 0; i < blocksStarts.Count() - 1; i++)
            {
                var startIndex = blocksStarts[i];
                var endIndex = blocksStarts[i + 1];
                var blockLines = lines.Skip(startIndex)
                                      .Take(endIndex - startIndex)
                                      .Where(x => x.Trim(' ', '\t') != string.Empty)
                                      .ToList();
                if (blockLines.Count() < 4) continue;

                codebase.AddBacktrace(blockLines, blocksStarts[i], blocksStarts[i + 1]);

                if (onProgress != null) onProgress((int)(50 + 50*((double)blocksStarts[i + 1] / lines.Count())));
            }
            codebase.Normalize();

            return codebase;
        }
    }
}
