using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace CloudinaryTest.Entities
{
    public class CloudFolder 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int Level { get; set; }
        public bool IsLeaf { get; set; }
        public long? ParentId { get; set; }
        public string Code { get; set; }
        public string CombineName { get; set; }
        public ICollection<CloudFile> CloudFiles { get; set; }
    }
}
