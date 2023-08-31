using CloudinaryTest.Entities;

namespace CloudinaryTest.Interfaces
{
    public interface ICloudFileRepository
    {
        ICollection<CloudFile> GetAll();
        CloudFile GetFile(long id);
        bool CloudFileExists(long id);
        bool CreateFile(CloudFile cloudFile);
        bool UpdateFile(CloudFile cloudFile);
        bool DeleteFile(long id);
        bool Save();
    }
}
