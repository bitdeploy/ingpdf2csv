using Bitdeploy.INGPdf2Csv.Model;
using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

namespace Bitdeploy.INGPdf2Csv
{
    public static class PdfParser
    {
        public static IEnumerable<Model.Transaction> ExtractTransactions(FileInfo fileInfo) => ExtractTransactions(fileInfo, new ApplicationOptions());

        public static IEnumerable<Model.Transaction> ExtractTransactions(FileInfo fileInfo, ApplicationOptions options)
        {
            if (fileInfo is null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
                                                    
            using var document = PdfDocument.Open(fileInfo.FullName, new ParsingOptions() { ClipPaths = true });
                        
            if (options.Verbose)
            {
                Console.WriteLine($"file: {fileInfo.FullName}");
                Console.WriteLine($"  number of pages: {document.NumberOfPages}");
            }

            if (options.Debug)
            {
                WriteDebugFile(fileInfo, options, document);
            }

            var objectExtractor = new ObjectExtractor(document);
            var pageIterator = objectExtractor.Extract();

            while (pageIterator.MoveNext())
            {
                var pageArea = pageIterator.Current;

                if (options.Verbose)
                {
                    Console.WriteLine($"  parsing page: {pageArea.PageNumber}");
                }

                var pageSubArea = pageArea.PageNumber == 1 ? 
                    pageArea.GetArea(options.FirstPageArea.ToFloatArray().ToPdfRectangle()) : 
                    pageArea.GetArea(options.OtherPageArea.ToFloatArray().ToPdfRectangle());

                var basicExtractionAlgorithm = new BasicExtractionAlgorithm();
                var tables = basicExtractionAlgorithm.Extract(pageSubArea, options.VerticalGuideLines.ToFloatArray());

                if (tables.Count == 0)
                {
                    if (options.Verbose)
                    {
                        Console.WriteLine($"  no table found on page");
                    }

                    continue;
                }

                var table = tables.First();

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var t = new Model.Transaction();

                    if ((!String.IsNullOrEmpty(table.Rows[i][0].GetText()) &&
                        String.IsNullOrEmpty(table.Rows[i][1].GetText()) &&
                        String.IsNullOrEmpty(table.Rows[i][2].GetText())) ||
                        (table.Rows[i][0].GetText() == "Buchung") ||
                        (table.Rows[i][0].GetText() == "Valuta"))
                    {
                        if (options.Verbose)
                        {
                            Console.WriteLine($"  skipping header row");
                        }

                        continue;
                    }

                    if (table.Rows[i][1].GetText() == "Neuer Saldo")
                    {
                        if (options.Verbose)
                        {
                            Console.WriteLine($"  found 'Neuer Saldo', skipping other pages");
                        }

                        yield break;
                    }

                    // 1st row
                    t.TransactionDate = DateOnly.Parse(table.Rows[i][0].GetText());
                    t.TransactionOther = table.Rows[i][1].GetText();
                    t.Amount = Decimal.Parse(table.Rows[i][2].GetText());

                    // 2nd row
                    i++;
                    t.ValutaDate = DateOnly.Parse(table.Rows[i][0].GetText());
                    t.Purpose = table.Rows[i][1].GetText();

                    // multi line purpose
                    while (i + 1 < table.Rows.Count && String.IsNullOrEmpty(table.Rows[i + 1][0].GetText()) && String.IsNullOrEmpty(table.Rows[i + 1][2].GetText()))
                    {
                        i++;
                        t.Purpose += (options.MultiLine ?  "\n" : " ") + table.Rows[i][1].GetText();
                    }

                    yield return t;
                }
            }
        }

        private static void WriteDebugFile(FileInfo fileInfo, ApplicationOptions options, PdfDocument document)
        {
            var debugFolder = new DirectoryInfo(Path.Combine(fileInfo.DirectoryName!, "Debug"));
            var debugFile = new FileInfo(Path.Combine(debugFolder.FullName, fileInfo.Name));

            if (!debugFolder.Exists)
            {
                debugFolder.Create();
            }
            
            var builder = new PdfDocumentBuilder();            
                                                
            for (int pageNumber = 1; pageNumber <= document.NumberOfPages; pageNumber++)
            {
                var page = builder.AddPage(document, pageNumber);
                page.SetStrokeColor(255, 0, 0);

                var pageArray = pageNumber == 1 ? options.FirstPageArea.ToDecimalArray(): options.OtherPageArea.ToDecimalArray();

                page.DrawRectangle(
                    new PdfPoint(pageArray[0], pageArray[1]),
                    pageArray[2] - pageArray[0],
                    pageArray[3] - pageArray[1],
                    0.5m);

                foreach (var guideLine in options.VerticalGuideLines.ToDecimalArray())
                {
                    page.DrawLine(
                        new PdfPoint(guideLine, pageArray[1]),
                        new PdfPoint(guideLine, pageArray[3]),
                        0.5m);
                }
            }

            File.WriteAllBytes(debugFile.FullName, builder.Build());

            if (options.Verbose)
            {
                Console.WriteLine($"  created debug document: {debugFile.FullName}");
            }
        }
    }
}
