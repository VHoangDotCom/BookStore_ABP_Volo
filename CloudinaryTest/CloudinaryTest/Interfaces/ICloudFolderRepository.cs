using Abp.Domain.Repositories;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using System.Collections;

namespace CloudinaryTest.Interfaces
{
    public interface ICloudFolderRepository
    {
        ICollection<CloudFolder> GetAll();
        Task<ICollection<CloudFolder>> GetAllAsync();
        CloudFolder GetFolder(long id);
        Task<CloudFolder> GetFolderAsync(long id);
        CloudFolder GetFolderTrimToUpper(CloudFolderDto cloudFolder);
        void UpdateRange(List<CloudFolder> cloudFolders);
        bool FolderExists(long id);
        bool CreateFolder(CloudFolder folder);
        bool UpdateFolder(CloudFolder folder);
        Task<bool> UpdateFolderAsync(CloudFolder folder);
        bool DeleteFolder(long id);
        Task<bool> DeleteFolderAsync(long id);
        bool Save();

    }
}
