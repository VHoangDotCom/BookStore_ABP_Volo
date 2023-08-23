using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.BookStore.CloudinaryFiles
{
    public class CloudinaryFileAppService : BookStoreAppService, ICloudinaryFileAppService
    {
        private readonly CloudinaryFileManager _cloudinaryFileManager;

        public CloudinaryFileAppService(
            CloudinaryFileManager cloudinaryFileManager
            )
        {
            _cloudinaryFileManager = cloudinaryFileManager;
        }

        public Task<string> DeleteAllAssetsInFolder(string folder)
        {
            return _cloudinaryFileManager.DeleteAllAssetsInFolder(folder);
        }

        public Task<string> DeleteAsset(string publicId, string folder)
        {
           return _cloudinaryFileManager.DeleteAsset( publicId, folder);
        }

        public Task<string> GetSingleAsset(string publicId, string folder)
        {
            return _cloudinaryFileManager.GetSingleAsset(publicId, folder);
        }

        public Task<string> UpdateAsset(string folder, string publicId, string newImagePath)
        {
            return _cloudinaryFileManager.UpdateAsset(folder, publicId, newImagePath);
        }

        public Task<string> UploadFile(int level, List<string> folders, string imagePath)
        {
            return _cloudinaryFileManager.UploadFile(level, folders, imagePath);
        }

        public Task<string> UploadMultipleFiles(string folder)
        {
            return _cloudinaryFileManager.UploadMultipleFiles(folder);
        }
    }
}
