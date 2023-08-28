namespace CloudinaryTest.Entities
{
    public enum FormatType
    {
        JPG = 0,
        JPGE = 1,
        GIF = 2,
        PNG = 3,
        JPE = 4,
        TIF = 5
    }
    public class CloudFile
    {
        public Guid Id { get; set; }
        public string PublicId { get; set; }
        public FormatType Format { get; set; }
        public string ImageURL { get; set; }
        public string ImagePath { get; set; }
        public bool IsOverride { get; set; }
        public string FolderPath { get; set; }
        public long FolderId { get; set; }
        public CloudFolder CloudFolder { get; set; }
    }
}
