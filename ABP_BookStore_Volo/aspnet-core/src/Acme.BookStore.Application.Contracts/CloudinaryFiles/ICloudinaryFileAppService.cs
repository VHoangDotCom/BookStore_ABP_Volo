using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.CloudinaryFiles
{
    public interface ICloudinaryFileAppService : IApplicationService
    {
        Task<string> UploadFile(int level, List<string> folders, string imagePath);
        Task<string> GetSingleAsset(string publicId, string folder);
        Task<string> UploadMultipleFiles(string folder);
        Task<string> DeleteAsset(string publicId, string folder);
        Task<string> DeleteAllAssetsInFolder(string folder);
        Task<string> UpdateAsset(string folder, string publicId, string newImagePath);
    }
}
