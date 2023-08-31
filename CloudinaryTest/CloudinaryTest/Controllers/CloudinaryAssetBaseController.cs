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
        #endregion

    }
}
