using System;
using CommandLine;
using CommandLine.Text;
using TableML.Compiler;

namespace TableCompilerConsole
{

    class Option
    {
        [Option('s', "Src", Required = true,
            HelpText = "Input excel or tsv files directory to be processed.")]
        public string Directory { get; set; }

        [Option('t', "To", Required = true,
            HelpText = "Output compiled files")]
        public string OutputDirectory { get; set; }

        [Option('c', "CodeFile", Required = false,
            HelpText = "code file")]
        public string CodeFilePath { get; set; }

        [Option("TemplateFile", Required = false,
            HelpText = "code generate template file")]
        public string TemplateFilePath { get; set; }


        [Option('v', "verbose", DefaultValue = true,
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
    class TableCompilerConsole
    {
        public static void Main(string[] args)
        {
            var options = new Option();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Values are available here
                if (options.Verbose) Console.WriteLine("Filename: {0}", options.Directory);

                var batchCompiler = new BatchCompiler();

                string templateString = DefaultTemplate.GenCodeTemplate;
                if (!string.IsNullOrEmpty(options.TemplateFilePath))
                {
                    Console.WriteLine(options.TemplateFilePath);
                    templateString = System.IO.File.ReadAllText(options.TemplateFilePath);

                }

                //var results =
                batchCompiler.CompileTableMLAll(options.Directory, options.OutputDirectory, options.CodeFilePath,
                   templateString, "AppSettings", ".tml", null, !string.IsNullOrEmpty(options.CodeFilePath));

                Console.WriteLine("Done!");

                //				var compiler = new Compiler();
                //				var result = compiler.Compile(options.Directory);
                //				Console.WriteLine(string.Format("Compile excel file: {0} , to {1}", options.Directory, result.TabFileRelativePath));
            }
        }
    }
}
