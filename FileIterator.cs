namespace Bitdeploy.INGPdf2Csv
{
    public static class FileIterator
    {
        public static IEnumerable<FileInfo> IterateFiles(string folderName,string searchPattern) => IterateFiles(new DirectoryInfo(folderName), searchPattern);

        public static IEnumerable<FileInfo> IterateFiles(DirectoryInfo folderName, string searchPattern)
        {            
            if(!folderName.Exists)
            {
                throw new ArgumentException($"folder '{folderName}' not found or accessable");
            }

            var separators = new char[] {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            };

            foreach (var fileInfo in folderName.GetFiles(searchPattern, 
                new EnumerationOptions 
                { 
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    MatchCasing = MatchCasing.CaseInsensitive
                }))
            {
                if (fileInfo.Directory!.FullName.Split(separators).Any(x => x.Equals("Debug")))
                    continue;

                yield return fileInfo;
            }
        }
    }
}
