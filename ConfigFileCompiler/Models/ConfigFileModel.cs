namespace ConfigFileComparer.Models
{
    public class ConfigFileModel
    {
        public string OldFilePath { get; set; }
        public string NewFilePath { get; set; }
        public string OldFileContent { get; set; }
        public string NewFileContent { get; set; }
        public string ComparisonResult { get; set; }
    }
}