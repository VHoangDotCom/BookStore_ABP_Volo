using Abp.Dependency;

namespace CloudinaryTest.Dto
{
    public class CloudFolderDto
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Level { get; set; }
        public bool IsLeaf { get; set; }
        public long? ParentId { get; set; }
        public string Code { get; set; }
        public string CombineName { get; set; }
    }

    public class CreateCloudFolderDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public long? ParentId { get; set; }
    }

    public class UpdateCloudFolderDto
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class InputToGetFolderDto
    {
        public bool? IsActive { get; set; }
        public bool? IsLeaf { get; set; }
        public string? SearchText { get; set; }

        public bool IsGetAll()
        {
            return !IsActive.HasValue && string.IsNullOrEmpty(SearchText) && !IsLeaf.HasValue;
        }
    }
}
