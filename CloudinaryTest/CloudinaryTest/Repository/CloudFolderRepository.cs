using CloudinaryTest.DAL;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using CloudinaryTest.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudinaryTest.Repository
{
    public class CloudFolderRepository : ICloudFolderRepository
    {
        private readonly CloudDBContext _dbContext;
        public CloudFolderRepository(CloudDBContext context)
        {
            _dbContext = context;
        }

        public bool CreateFolder(CloudFolder folder)
        {
            _dbContext.Add(folder);
            return Save();
        }

        public bool DeleteFolder(long id)
        {
            var folder = _dbContext.CloudFolders.Where(x => x.Id == id).FirstOrDefault();
            if(folder != null)
            {
                _dbContext.Remove(folder);
            }
            else { return false; }
            return Save();
        }

        public async Task<bool> DeleteFolderAsync(long id)
        {
            var folder = await _dbContext.CloudFolders.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (folder != null)
            {
                _dbContext.Remove(folder);
            }
            else { return false; }
            return Save();
        }

        public bool FolderExists(long id)
        {
            return _dbContext.CloudFolders.Any(p => p.Id == id);
        }

        public ICollection<CloudFolder> GetAll()
        {
            return _dbContext.CloudFolders.OrderBy(x => x.Name).ToList();
        }

        public async Task<ICollection<CloudFolder>> GetAllAsync()
        {
            return await _dbContext.CloudFolders.OrderBy(x => x.Name).ToListAsync();
        }

        public CloudFolder GetFolder(long id)
        {
            return _dbContext.CloudFolders.Where(x => x.Id == id).FirstOrDefault();
        }

        public async Task<CloudFolder> GetFolderAsync(long id)
        {
            return await _dbContext.CloudFolders.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public CloudFolder GetFolderTrimToUpper(CloudFolderDto cloudFolder)
        {
            return GetAll()
                .Where(cl => cl.Name.Trim().ToUpper() == cloudFolder.Name.TrimEnd().ToUpper())
                .FirstOrDefault();
        }

        public bool Save()
        {
            var saved = _dbContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateFolder(CloudFolder folder)
        {
            var folderUpdate = _dbContext.CloudFolders.Where(x => x.Id == folder.Id).FirstOrDefault();

            if(folderUpdate != null)
            {
                folderUpdate.Name = folder.Name;
                folderUpdate.Code = folder.Code;

                _dbContext.Update(folderUpdate);
                return  Save();
            }

            return false;
        }

        public async Task<bool> UpdateFolderAsync(CloudFolder folder)
        {
            var folderUpdate = await _dbContext.CloudFolders.Where(x => x.Id == folder.Id).FirstOrDefaultAsync();

            if (folderUpdate != null)
            {
                folderUpdate.Name = folder.Name;
                folderUpdate.Code = folder.Code;

                _dbContext.Update(folderUpdate);
                return Save();
            }

            return false;
        }

        public void UpdateRange(List<CloudFolder> cloudFolders)
        {
            _dbContext.UpdateRange(cloudFolders);
        }
    }
}
