namespace Bitdeploy.INGPdf2Csv.Model
{
    public class ApplicationOptions
    {
        public DirectoryInfo ImportFolder { get; set; } = null!;
        public FileInfo OutputFile { get; set; } = null!;
        public string SearchPattern { get; set; } = null!;

        public string VerticalGuideLines { get; set; } = null!;
        public string FirstPageArea { get; set; } = null!;
        public string OtherPageArea { get; set; } = null!;

        public bool MultiLine { get; set; }
        public bool Verbose { get; set; }
        public bool Debug { get; set; }
    }
}
