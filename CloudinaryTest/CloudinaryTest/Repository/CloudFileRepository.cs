using CloudinaryTest.DAL;
using CloudinaryTest.Entities;
using CloudinaryTest.Interfaces;

namespace CloudinaryTest.Repository
{
    public class CloudFileRepository : ICloudFileRepository
    {
        private readonly CloudDBContext _dbContext;
        public CloudFileRepository(CloudDBContext context)
        {
            _dbContext = context;
        }

        public ICollection<CloudFile> GetAll()
        {
            return _dbContext.CloudFiles.ToList();
        }

        public CloudFile GetFile(long id)
        {
            return _dbContext.CloudFiles.Where(e => e.Id == id).FirstOrDefault();
        }

        public bool CloudFileExists(long id)
        {
            return _dbContext.CloudFiles.Any(c => c.Id == id);
        }

        public bool CreateFile(CloudFile cloudFile)
        {
            _dbContext.Add(cloudFile);
            return Save();
        }

        public bool UpdateFile(CloudFile cloudFile)
        {
            var fileUpdate = _dbContext.CloudFiles.Where(x => x.Id == cloudFile.Id).FirstOrDefault();

            if (fileUpdate != null)
            {
                fileUpdate.PublicId = cloudFile.PublicId;
                fileUpdate.Format = cloudFile.Format;
                //fileUpdate.ImageURL = cloudFile.ImageURL;
                fileUpdate.ImagePath = cloudFile.ImagePath;
                fileUpdate.IsOverride = cloudFile.IsOverride;
                //fileUpdate.FolderPath = cloudFile.FolderPath;
                fileUpdate.FolderId = cloudFile.FolderId;
            }

            return Save();
        }

        public bool DeleteFile(long id)
        {
            var file = _dbContext.CloudFiles.Where(x => x.Id == id).FirstOrDefault();
            _dbContext.Remove(file);
            return Save();
        }

        public bool Save()
        {
            var saved = _dbContext.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
