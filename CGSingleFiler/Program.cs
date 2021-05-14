using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CGSingleFiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("First argument should be a path to project root (containing .csproj)");
                Console.WriteLine("Second argument should be a path to merged file");
                return;
            }
            var dir = new DirectoryInfo(args[0]);
            if (!dir.Exists)
            {
                Console.WriteLine($"Directory {dir.FullName} does not exist");
                return;
            }

            if (dir.GetFiles().Any(f => f.Extension == ".csproj") == false)
            {
                Console.WriteLine($"Directory {dir.FullName} does not contain .csproj file");
                return;
            }

            var output = new FileInfo(args[1]);
            if (!output.Directory.Exists)
            {
                Console.WriteLine($"Directory {output.Directory.FullName} does not exist");
                return;
            }

            Job(dir, output);
        }

        private static void Job(DirectoryInfo projectDir, FileInfo outputFile)
        {
            var sourceFiles = new List<SourceFile>();
            ProcessDir(projectDir, sourceFiles);

            var usingLines = sourceFiles.SelectMany(s => s.Usings).Distinct();
            var namespaceLines = sourceFiles.SelectMany(s => new List<string> { "\n" }.Concat(s.NamespaceLines));

            File.WriteAllLines(outputFile.FullName, usingLines.Concat(namespaceLines).ToArray());
        }

        private static void ProcessDir(DirectoryInfo dir, List<SourceFile> sourceFiles)
        {
            foreach (var subdir in dir.EnumerateDirectories())
                ProcessDir(subdir, sourceFiles);

            foreach (var file in dir.EnumerateFiles())
            {
                if (file.Extension != ".cs")
                    continue;
                sourceFiles.Add(new SourceFile(file));
            }
        }
    }
}
