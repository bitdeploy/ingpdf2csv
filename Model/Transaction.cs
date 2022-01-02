using System.Text;

namespace Bitdeploy.INGPdf2Csv.Model;

public class Transaction
{
    public DateOnly TransactionDate { get; set; }
    public DateOnly ValutaDate { get; set; }
    public string TransactionOther { get; set; } = default!;
    public string Purpose { get; set; } = default!;
    public decimal Amount { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("TransactionDate:".PadRight(20) + TransactionDate);
        sb.AppendLine("Buchung:".PadRight(20) + TransactionOther);
        sb.AppendLine("Betrag:".PadRight(20) + Amount.ToString("n2"));

        return sb.ToString();
    }
}

