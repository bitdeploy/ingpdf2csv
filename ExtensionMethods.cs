using UglyToad.PdfPig.Core;

namespace Bitdeploy.INGPdf2Csv
{
    public static class ExtensionMethods
    {
        public static float[] ToFloatArray(this string input)
        {
            return input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToSingle(x))
                .ToArray();
        }

        public static decimal[] ToDecimalArray(this string input)
        {
            return input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToDecimal(x))
                .ToArray();
        }

        public static PdfRectangle ToPdfRectangle(this float[] input)
        {
            return new PdfRectangle(
                input[0],
                input[1],
                input[2],
                input[3]);
        }
    }
}
