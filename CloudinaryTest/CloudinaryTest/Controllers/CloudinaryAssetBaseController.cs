using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CloudinaryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryAssetBaseController : ControllerBase
    {
        public static Cloudinary cloudinary;

        public const string CLOUD_NAME = "dduv8pom4";
        public const string API_KEY = "952444439587681";
        public const string API_SECRET = "ubB0ir_v5YXR4KxmnZnuQHORoew";

        #region Cloud File
        [HttpGet("GetSingleAsset")]
        public async Task<IActionResult> GetAsset()
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            string folder = "test_level_1/test_level_2";
            string publicId = "dog";

            GetResourceResult resourceResult = cloudinary.GetResource($"{folder}/{publicId}");

            if (resourceResult.StatusCode == HttpStatusCode.OK)
            {
                string imageUrl = resourceResult.SecureUrl.ToString();
                return Ok(new { Message = $"Asset found: {resourceResult.PublicId}", ImageUrl = imageUrl });
            }
            else
            {
                return BadRequest("Asset not found.");
            }
        }

        [HttpGet("CheckFileExistedInFolder")]
        public async Task<IActionResult> CheckFileExistedInFolder(string publicID, string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            string assetPublicId = $"{folder}/{publicID}";

            //folder is a Combine Name of CloudFolder
            SearchResult result = cloudinary.Search()
                  .Expression($"folder={folder} AND public_id={assetPublicId}")
                  .Execute();

            /*
             * Some expression:
                created_at<=2023-01-01
                last_updated

            	format=(jpg OR mp4)

            	height<=100
                width>100
                width=1028

            	resource_type:image
                resource_type:video

            	status=deleted
                status=deleted OR active

                public_id:[a TO mz]

                assets containing 'cat' in any field but not 'kitten' in the tags field
                cat AND -tags:kitten
             */

            if (result.Resources != null && result.Resources.Count > 0)
            {
                return Ok("File exists.");
            }
            else
            {
                return BadRequest("File does not exist.");
            }
        }

        [HttpPost("UploadFile")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UploadFile(int level, List<string> folders, string imagePath)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            cloudinary = new Cloudinary(account);
            try
            {
                if (folders.Count != level)
                {
                    return BadRequest("Warning: The number of folder strings exceeds the specified level.");
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
                return Ok("[Image was uploaded successfully]");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("UploadMultipleFiles")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UploadMultipleFiles()
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            try
            {
                List<string> imagePaths = new List<string>
                    {
                        "C:/theme/thresh.jpg",
                        "C:/theme/dragon.jpg",
                        "C:/theme/dog.jpg"
                    };

                string folder = "test_level_1/test_level_1.1";

                List<string> uploadResults = new List<string>(); // To store upload result messages

                foreach (var imagePath in imagePaths)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imagePath),
                        PublicId = Path.GetFileNameWithoutExtension(imagePath), // Use image name as PublicId
                        Folder = folder
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);

                    if (uploadResult.StatusCode == HttpStatusCode.OK)
                    {
                        uploadResults.Add($"Image '{imagePath}' uploaded successfully.");
                    }
                    else
                    {
                        uploadResults.Add($"Failed to upload image '{imagePath}'.");
                    }
                }

                if (uploadResults.All(result => result.Contains("uploaded successfully")))
                {
                    return Ok(uploadResults); // All images uploaded successfully
                }
                else
                {
                    return BadRequest(uploadResults); // At least one image failed to upload
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("UpdateAsset")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateAsset(string folder, string publicId, string newImagePath)
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
                return Ok("Asset updated successfully.");
            }
            else
            {
                return StatusCode(500, "Failed to update asset.");
            }
        }

        [HttpDelete("DeleteAsset")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteAsset(string publicId, string folder)
        {
            //Delete specific asset
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            //folder must be based on level
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
                return Ok("Asset deleted successfully.");
            }
            else
            {
                // Failed to delete asset
                return StatusCode(500, "Failed to delete asset.");
            }
        }

        [HttpDelete("DeleteMultipleAssets")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteMultipleAssets(string folder)
        {
            //Delete all assets
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            var delRes = cloudinary.DeleteResourcesByPrefix(folder);

            if (delRes != null && delRes.Deleted != null && delRes.Deleted.Count > 0)
            {
                return Ok($"{delRes.Deleted.Count} assets deleted in folder {folder}.");
            }
            else
            {
                return Ok($"No assets found in folder {folder}.");
            }
        }

        #endregion


        #region Cloud Folder

        [HttpGet("CheckExistedFolder")]
        public async Task<IActionResult> CheckFolderExistence(string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            SearchResult result = cloudinary.Search()
                  .Expression($"folder={folder}")
                  .Execute();

            if (result.Resources != null && result.Resources.Count > 0)
            {
                return Ok("Folder exists.");
            }
            else
            {
                return BadRequest("Folder does not exist.");
            }
        }

        [HttpGet("CheckAndListExistedAssetsInFolder")]
        public async Task<IActionResult> CheckAndListExistedAssetsInFolder(string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            SearchResult result = cloudinary.Search()
                .Expression($"folder:{folder}")
                .Execute();

            var subFoldersResult = cloudinary.SubFolders(folder);
            List<string> folderNames = subFoldersResult.Folders.Select(subFolder => subFolder.Name).ToList();

            if (result.Resources != null)
            {
                bool folderContainsItems = result.Resources.Any(resource =>
                    !string.IsNullOrWhiteSpace(resource.PublicId) || !string.IsNullOrWhiteSpace(resource.Folder));

                if (folderContainsItems)
                {
                    List<string> assetUrls = result.Resources
                        .Where(resource => !string.IsNullOrWhiteSpace(resource.PublicId))
                        .Select(resource => $"{resource.PublicId} - {resource.Url}")
                        .ToList();

                    if (assetUrls.Count > 0)
                    {
                        string assetUrlsString = string.Join("\n", assetUrls);
                        string folderNamesString = string.Join("\n", folderNames);

                        return Ok($"List assets:\n{assetUrlsString}\n\nSubFolder Names:\n{folderNamesString}");
                    }
                    else
                    {
                        return Ok("Folder does not contain any assets.");
                    }
                }
                else
                {
                    return Ok("Folder exists but is empty.");
                }
            }
            else
            {
                return BadRequest("Folder does not exist or contains no assets.");
            }
        }

        [HttpGet("GetAssetsInFolder")]
        public async Task<IActionResult> GetAssetsInFolder(string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            SearchResult result = cloudinary.Search()
                .Expression($"folder={folder}")
                .Execute();

            if (result.Resources != null && result.Resources.Count > 0)
            {
                List<string> assetUrls = result.Resources.Select(resource => resource.Url).ToList();
                return Ok(assetUrls);
            }
            else
            {
                return BadRequest("Folder does not exist or contains no assets.");
            }
        }

        [HttpGet("GetAllRootFolders")]
        public async Task<IActionResult> GetAllRootFolders()
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            // Use the RootFolders method to retrieve all root-level folders
            var rootFoldersResult = cloudinary.RootFolders();//Get root folders

            if (rootFoldersResult != null && rootFoldersResult.Folders != null)
            {
                // Extract folder names from the result
                List<string> folderNames = rootFoldersResult.Folders.Select(folder => folder.Name).ToList();
                return Ok(folderNames);
            }
            else
            {
                return BadRequest("No folders found.");
            }
        }

        [HttpGet("GetSubFolders")]
        public async Task<IActionResult> GetSubFolders(string folder)
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            var subFoldersResult = cloudinary.SubFolders(folder);//Get subfolders

            if (subFoldersResult != null && subFoldersResult.Folders != null)
            {
                // Extract folder names from the result
                List<string> folderNames = subFoldersResult.Folders.Select(folder => folder.Name).ToList();
                return Ok(folderNames);
            }
            else
            {
                return BadRequest("No folders found.");
            }
        }

        [HttpPost("CreateNewRootFolder")]
        public async Task<IActionResult> CreateNewRootFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return BadRequest("Folder name cannot be empty.");
            }

            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            // Retrieve the list of existing folders
            var existedFolder = await cloudinary.RootFoldersAsync();

            // Check if a folder with the same name already exists
            if (existedFolder.Folders.Any(existingFolder => existingFolder.Name == folder))
            {
                return BadRequest($"Folder '{folder}' already exists.");
            }

            // If the folder doesn't exist, create it
            await cloudinary.CreateFolderAsync(folder);
            return Ok("Folder created successfully!");
        }

        [HttpPost("CreateNewSubFolder")]
        public async Task<IActionResult> CreateNewSubFolder(int level, List<string> parentFolders, string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return BadRequest("Folder name cannot be empty.");
            }

            if (level <= 0 || parentFolders.Count != level)
            {
                return BadRequest("Invalid level or parent folder list.");
            }

            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            // Construct the full folder path based on level and parent folders
            string fullFolderPath = string.Join("/", parentFolders.Take(level)) + "/" + folder;

            // Retrieve the list of existing folders
            var existedFolder = await cloudinary.RootFoldersAsync();

            // Check if a folder with the same name already exists
            if (existedFolder.Folders.Any(existingFolder => existingFolder.Path == fullFolderPath))
            {
                return BadRequest($"Folder '{fullFolderPath}' already exists.");
            }

            // If the folder doesn't exist, create it
            await cloudinary.CreateFolderAsync(fullFolderPath);
            return Ok($"Folder '{fullFolderPath}' created successfully!");
        }

        [HttpDelete("DeleteRootFolder")]
        public async Task<IActionResult> DeleteRootFolder(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return BadRequest("Folder name cannot be empty.");
            }

            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            // Retrieve the list of existing folders
            var existedFolder = await cloudinary.RootFoldersAsync();

            // Check if a folder with the same name already exists
            if (existedFolder.Folders.Any(existingFolder => existingFolder.Name == folder))
            {
                cloudinary.DeleteFolder(folder);
                return Ok($"Folder '{folder}' is deleted successfully!");
            }
            else
            {
                return BadRequest($"Folder '{folder}' is not existed!");
            }
        }

        [HttpDelete("DeleteSubfolder")]
        public async Task<IActionResult> DeleteSubfolder(string subfolder)
        {
            if (string.IsNullOrWhiteSpace(subfolder))
            {
                return BadRequest("Subfolder path cannot be empty.");
            }

            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            Cloudinary cloudinary = new Cloudinary(account);

            // Construct the full path to the subfolder
            string fullSubfolderPath = $"test_level_3/test_level_3.1/test_level_3.1.1/{subfolder}";

            // Attempt to delete the subfolder
            var result = cloudinary.DeleteFolder(fullSubfolderPath);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok($"Subfolder '{subfolder}' is deleted successfully!");
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return BadRequest($"Subfolder '{subfolder}' does not exist.");
            }
            else
            {
                return StatusCode((int)result.StatusCode, $"Failed to delete subfolder '{subfolder}'.");
            }
        }

        [HttpPut("UpdateFolder")]
        public async Task<IActionResult> UpdateFolder(string currentFolderName, string newFolderName)
        {
            //1 - Create new folder with the new name - change it combine path also
            // In DB local, just update the name of that folder - Done
            //2 - Move subfolders of old one to new one (update their combineName actually)
            // Both DB local and Cloud
            //3 - Rename publicId of each assets (both in cloud and local)
            //4 - Delete old folder in cloud

            return Ok("");
        }

        #endregion

    }
}
