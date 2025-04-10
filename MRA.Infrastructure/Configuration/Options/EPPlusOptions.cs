namespace MRA.Infrastructure.Configuration.Options;

public class EPPlusOptions
{
    public ExcelPackageOptions ExcelPackage { get; set; }
    public FileOptions File { get; set; }

    public class ExcelPackageOptions
    {
        public string LicenseContext { get; set; }
    }

    public class FileOptions
    {
        public string Name { get; set; }
        public string DateFormat { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
    }
}