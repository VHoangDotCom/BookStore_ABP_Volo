using CloudinaryTest.Entities;

namespace CloudinaryTest.Dto
{
    public class CloudFileDto
    {
        public long Id { get; set; }
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
