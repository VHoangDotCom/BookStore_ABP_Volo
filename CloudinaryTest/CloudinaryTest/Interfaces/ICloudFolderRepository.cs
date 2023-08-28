using Abp.Domain.Repositories;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using System.Collections;

namespace CloudinaryTest.Interfaces
{
    public interface ICloudFolderRepository
    {
        ICollection<CloudFolder> GetAll();
        CloudFolder GetFolder(long id);
        CloudFolder GetFolderTrimToUpper(CloudFolderDto cloudFolder);
        void UpdateRange(List<CloudFolder> cloudFolders);
        bool FolderExists(long id);
        bool CreateFolder(CloudFolder folder);
        bool UpdateFolder(CloudFolder folder);
        bool DeleteFolder(long id);
        bool Save();

    }
}
