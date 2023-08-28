using CloudinaryTest.Common.Models;

namespace CloudinaryTest.Dto
{
    public class TreeFolderDto
    {
        public long Id => 0;
        public string Code => "Root";
        public string Name => "Root";
        public bool IsLeaf => false;
        public IEnumerable<TreeItem<CloudFolderDto>> Childrens { get; set; }
    }
}
