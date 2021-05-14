using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CGSingleFiler
{
    class SourceFile
    {
        public List<string> Usings { get; }
        public List<string> NamespaceLines { get; }
        public SourceFile(FileInfo fileInfo)
        {
            Usings = new List<string>();
            NamespaceLines = new List<string>();

            var lines = File.ReadAllLines(fileInfo.FullName);
            var namespaceStarted = false;

            foreach (var line in lines)
            {
                if (namespaceStarted)
                {
                    NamespaceLines.Add(line);
                    continue;
                }

                if (line == string.Empty)
                    continue;

                if (line.StartsWith("using"))
                {
                    Usings.Add(line);
                } else if (line.StartsWith("namespace"))
                {
                    namespaceStarted = true;
                    NamespaceLines.Add(line);
                }
            }
        }
    }
}
