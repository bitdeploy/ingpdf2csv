using Bitdeploy.INGPdf2Csv.Model;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace Bitdeploy.INGPdf2Csv
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Console.WriteLine(e.ExceptionObject.ToString());
                Environment.Exit(1);
            };

            var rootCommand = new RootCommand
            {
                new Argument<DirectoryInfo>("importFolder", "Import folder name") {Arity = ArgumentArity.ExactlyOne},
                new Option<FileInfo>(new string[] {"--outputFile", "-o" }, () => new FileInfo("export.csv"), "Export file name") {Arity = ArgumentArity.ZeroOrOne},
                new Option<string>(new string[] {"--searchPattern", "-s" }, () => "*.pdf", "Search pattern") {Arity = ArgumentArity.ZeroOrOne},
                new Option<string>(new string[] {"--verticalGuideLines" }, () => "140,480", "Vertical guidelines in the table") {Arity = ArgumentArity.ZeroOrOne},
                new Option<string>(new string[] {"--firstPageArea" }, () => "60,80,560,475", "First page table area (bottom left x, bottom left y, top right x, top right y)") {Arity = ArgumentArity.ZeroOrOne},
                new Option<string>(new string[] {"--otherPageArea" }, () => "60,70,560,630", "First page table area (bottom left x, bottom left y, top right x, top right y)") {Arity = ArgumentArity.ZeroOrOne},
                new Option<bool>(new string[] {"--multiLine" }, "Use multi line text with line breaks") {Arity = ArgumentArity.ZeroOrOne},
                new Option<bool>(new string[] {"--verbose" }, "Enable verbose mode") {Arity = ArgumentArity.ZeroOrOne},
                new Option<bool>(new string[] {"--debug" }, "Enable debug mode, creates a copy of the imported file with table parsing guidelines") {Arity = ArgumentArity.ZeroOrOne}
            };

            rootCommand.Description = "ING Pdf2Csv - converts ING pdf files to csv";
            rootCommand.Handler = CommandHandler.Create((ApplicationOptions options) => ApplicationCommandHandler(options));

            return await rootCommand.InvokeAsync(args);
        }

        private static void ApplicationCommandHandler(ApplicationOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Verbose)
            {
                Console.WriteLine($"importFolder: {options.ImportFolder?.FullName}");
                Console.WriteLine($"outputFile: {options.OutputFile?.FullName}");
                Console.WriteLine($"searchPattern: {options.SearchPattern}");
                Console.WriteLine($"verticalGuideLines: {string.Join(",", options.VerticalGuideLines)}");
            }

            if (options.OutputFile!.Exists)
            {
                options.OutputFile.Delete();
            }

            foreach (var pdfFile in FileIterator.IterateFiles(options.ImportFolder!, options.SearchPattern))
            {
                CsvWriter.Write(
                    PdfParser.ExtractTransactions(pdfFile, options),
                    options.OutputFile);
            }
        }
    }
}