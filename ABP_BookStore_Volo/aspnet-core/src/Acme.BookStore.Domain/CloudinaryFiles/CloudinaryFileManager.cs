using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Acme.BookStore.CloudinaryFiles
{
    public class CloudinaryFileManager : DomainService
    {
        public static Cloudinary cloudinary;

        public const string CLOUD_NAME = "dduv8pom4";
        public const string API_KEY = "952444439587681";
        public const string API_SECRET = "ubB0ir_v5YXR4KxmnZnuQHORoew";

        public async Task<string> UploadFile(int level, List<string> folders, string imagePath)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            cloudinary = new Cloudinary(account);
            try
            {
                if (folders.Count > level)
                {
                    return "Warning: The number of folder strings exceeds the specified level.";
                }

                string folderStructure = string.Join("/", folders.Take(level));

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imagePath),
                    PublicId = "my_wife",
                    Overwrite = true,
                    Faces = true,
                    Folder = folderStructure
                    //Folder = "test_level_1/test_level_2"
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                return "[Image was uploaded successfully]";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> UploadMultipleFiles(string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            try
            {
                List<string> imagePaths = new List<string>
        {
            "C:/Users/Admin/Downloads/get_asset.png",
            "C:/Users/Admin/Downloads/upload_level1.png",
            "C:/Users/Admin/Downloads/upload_level2.png"
        };

                List<string> uploadResults = new List<string>();

                foreach (var imagePath in imagePaths)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imagePath),
                        PublicId = Path.GetFileNameWithoutExtension(imagePath),
                        Folder = folder
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == HttpStatusCode.OK)
                    {
                        uploadResults.Add($"Image '{Path.GetFileName(imagePath)}' uploaded successfully.");
                    }
                    else
                    {
                        uploadResults.Add($"Failed to upload image '{Path.GetFileName(imagePath)}'.");
                    }
                }

                if (uploadResults.All(result => result.Contains("uploaded successfully")))
                {
                    return string.Join("\n", uploadResults); // All images uploaded successfully
                }
                else
                {
                    return string.Join("\n", uploadResults); // At least one image failed to upload
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        public async Task<string> GetSingleAsset(string publicId, string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            GetResourceResult resourceResult = cloudinary.GetResource($"{folder}/{publicId}");

            if (resourceResult.StatusCode == HttpStatusCode.OK)
            {
                string secureUrl = resourceResult.SecureUrl.ToString();

                return $"Asset found: Public ID: {publicId}, URL: {secureUrl}";
            }
            else
            {
                return "Asset not found.";
            }
        }

        public async Task<string> DeleteAsset(string publicId, string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            string assetPublicId = $"{folder}/{publicId}";

            DeletionParams deletionParams = new DeletionParams(assetPublicId)
            {
                ResourceType = ResourceType.Image,
                Type = "upload"
            };

            DeletionResult deletionResult = cloudinary.Destroy(deletionParams);

            if (deletionResult.Result == "ok")
            {
                // Asset deleted successfully
                return("Asset deleted successfully.");
            }
            else
            {
                // Failed to delete asset
                return("Failed to delete asset.");
            }
        }

        public async Task<string> DeleteAllAssetsInFolder(string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            var delRes = cloudinary.DeleteResourcesByPrefix(folder);

            if (delRes != null && delRes.Deleted != null && delRes.Deleted.Count > 0)
            {
                return $"{delRes.Deleted.Count} assets deleted in folder {folder}.";
            }
            else
            {
                return $"No assets found in folder {folder}.";
            }
        }

        public async Task<string> UpdateAsset(string folder, string publicId, string newImagePath)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            string assetPublicId = $"{folder}/{publicId}";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(newImagePath),
                PublicId = assetPublicId,
                Overwrite = true
            };

            var uploadResult = cloudinary.Upload(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                return "Asset updated successfully.";
            }
            else
            {
                return "Failed to update asset.";
            }
        }

    }
}
