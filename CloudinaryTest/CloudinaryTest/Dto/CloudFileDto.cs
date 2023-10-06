using CloudinaryTest.Entities;

namespace CloudinaryTest.Dto
{
    public class GetCloudFileDto
    {
        public long Id { get; set; }
        public string PublicId { get; set; }
        public FormatType Format { get; set; }
        public string ImageURL { get; set; }
        public string ImagePath { get; set; }
        public bool IsOverride { get; set; }
        public string FolderPath { get; set; }
        public long FolderId { get; set; }
        public CloudFolderDto CloudFolder { get; set; }
    }

    public class CreateCloudFileDto
    {
        public string PublicId { get; set; }
        public FormatType Format { get; set; }
        public string ImagePath { get; set; }
        public bool IsOverride { get; set; }
        public long FolderId { get; set; }
    }

    public class CreateCloudinaryFileDto
    {
        public string PublicId { get; set; }
        public FormatType Format { get; set; }
        public string ImagePath { get; set; }
        public bool IsOverride { get; set; }
        public string FolderName { get; set; }
    }

    public class UpdateCloudinaryFileDto
    {
        public string PublicId { get; set; }
        public FormatType Format { get; set; }
        public string ImagePath { get; set; }
        public bool IsOverride { get; set; }
    }

    public class UpdateCloudFileDto
    {
        public long Id { get; set; }
        public FormatType Format { get; set; }
        public string ImagePath { get; set; }
        public bool IsOverride { get; set; }
    }
}
