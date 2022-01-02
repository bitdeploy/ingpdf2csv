using CsvHelper.Configuration;
using System.Globalization;

namespace Bitdeploy.INGPdf2Csv;
public static class CsvWriter
{
    public static void Write(IEnumerable<Model.Transaction> transactions, string exportFileName) => Write(transactions, new FileInfo(exportFileName));

    public static void Write(IEnumerable<Model.Transaction> transactions, FileInfo exportFileName)
    {
        exportFileName.Refresh();
        var fileExists = exportFileName.Exists;

        using var writer = new StreamWriter(exportFileName.FullName, true, System.Text.Encoding.UTF8);
        using var csv = new CsvHelper.CsvWriter(writer, 
            new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = !fileExists              
            });
        
        csv.Context.RegisterClassMap<TransactionMap>();       
        csv.WriteRecords(transactions);
        csv.Flush();        
    }

    public class TransactionMap : ClassMap<Model.Transaction>
    {
        public TransactionMap()
        {
            Map(m => m.TransactionDate).Index(0).Name("Buchungsdatum");
            Map(m => m.ValutaDate).Index(1).Name("Valutadatum");
            Map(m => m.TransactionOther).Index(2).Name("Buchung");
            Map(m => m.Purpose).Index(3).Name("Verwendungszweck");
            Map(m => m.Amount).Index(4).Name("Betrag (EUR)");
        }
    }
}

