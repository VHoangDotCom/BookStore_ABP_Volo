using Abp.UI;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryTest.Common.Manager;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using CloudinaryTest.Exceptions;
using CloudinaryTest.Helper;
using CloudinaryTest.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CloudinaryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudFileAdvanceController : Controller
    {
        private readonly ICloudFileRepository _cloudFileRepository;
        private readonly ICloudFolderRepository _cloudFolderRepository;
        private readonly IMapper _mapper;
        private readonly CommonManager _commonManager;

        private readonly Cloudinary _cloudinary;

        public const string CLOUD_NAME = "dduv8pom4";
        public const string API_KEY = "952444439587681";
        public const string API_SECRET = "ubB0ir_v5YXR4KxmnZnuQHORoew";

        public CloudFileAdvanceController(
            ICloudFileRepository repository,
            ICloudFolderRepository cloudFolderRepository,
            IMapper mapper,
            CommonManager commonManager
            )
        {
            _cloudFileRepository = repository;
            _cloudFolderRepository = cloudFolderRepository;
            _mapper = mapper;
            _commonManager = commonManager;

            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            _cloudinary = new Cloudinary(account);
        }

        #region LocalDB CloudFile

        [HttpGet("GetAll")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAll()
        {
            var files = _mapper.Map<List<GetCloudFileDto>>(_cloudFileRepository.GetAll());

            foreach (var file in files)
            {
                var folder = _mapper.Map<CloudFolderDto>(_cloudFolderRepository.GetFolder(file.FolderId));
                file.CloudFolder = folder;
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(files);
        }

        #endregion

        #region LocalDB-Cloud CloudFile

        [HttpGet("GetFile/{fileID}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetFile(long fileID)
        {
            if (!_cloudFileRepository.CloudFileExists(fileID))
                return NotFound();

            var category = _mapper.Map<GetCloudFileDto>(_cloudFileRepository.GetFile(fileID));

            category.CloudFolder = _mapper.Map<CloudFolderDto>(_cloudFolderRepository.GetFolder(category.FolderId));

            var isCloudExisted = CheckFileExistedInFolder(category.PublicId, category.FolderPath);
            if (!isCloudExisted)
            {
                return BadRequest("This file exists in DB but it does not existed in Cloudinary!");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpPost("CreateAndUploadFile")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateAndUploadFile([FromBody] CreateCloudFileDto fileCreate)
        {
            // PNG = 3
            var fileExisted = _cloudFileRepository.GetAll()
                .Where(c => c.PublicId.Trim().ToUpper() == fileCreate.PublicId.TrimEnd().ToUpper())
                .FirstOrDefault();

            var folderExisted = _cloudFolderRepository.GetFolder(fileCreate.FolderId);

            if (fileExisted != null) 
                throw new UserFriendlyException(String.Format($"File {fileExisted.PublicId} already exists"));

            if (folderExisted == null)
                throw new UserFriendlyException(String.Format($"Folder {fileCreate.FolderId} does not exists"));
          
            var fileMap = _mapper.Map<CloudFile>(fileCreate);

            fileMap.FolderPath = folderExisted.CombineName;

            var uploadCloudinaryFile = new CreateCloudinaryFileDto()
            {
                PublicId = fileCreate.PublicId,
                Format = fileCreate.Format,
                ImagePath = fileCreate.ImagePath,
                IsOverride = fileCreate.IsOverride,
                FolderName = fileMap.FolderPath
            };

            var cloudFileURL = await UploadFileCloudinary(uploadCloudinaryFile);
            if(cloudFileURL != null)
                fileMap.ImageURL = cloudFileURL;

            if (!_cloudFileRepository.CreateFile(fileMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Cloud File created successfully in DB and Cloudinary!");
        }

        [HttpPut("UpdateFile")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFile([FromBody] UpdateCloudFileDto updatedFile)
        {
            if (updatedFile == null)
                return BadRequest(ModelState);

            var existingFile = _cloudFileRepository.GetFile(updatedFile.Id);

            if (existingFile == null)
                return NotFound();
          
            //Update File in Cloudinary
            var oldCloudinaryPublicId = existingFile.FolderPath + "/" + existingFile.PublicId;

            var updateCloudinaryFile = new UpdateCloudinaryFileDto()
            {
                PublicId = oldCloudinaryPublicId,
                Format = updatedFile.Format,
                ImagePath = updatedFile.ImagePath,
                IsOverride = updatedFile.IsOverride
            };

            var cloudFileURL = await UpdateFileCloudinary(updateCloudinaryFile);
            if (cloudFileURL != null)
                existingFile.ImageURL = cloudFileURL;

            //Update File in Local DB
            existingFile.Format = updatedFile.Format;
            existingFile.ImagePath = updatedFile.ImagePath;
            existingFile.IsOverride = updatedFile.IsOverride;

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_cloudFileRepository.UpdateFile(existingFile))
            {
                ModelState.AddModelError("", "Something went wrong updating File");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("DeleteFile")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFile(long fileID)
        {
            var existedFile = _cloudFileRepository.GetFile(fileID);
            if (existedFile == null)
                throw new NotFoundException($"File with ID '{fileID}' not found.");

            var cloudPublicId = existedFile.FolderPath + "/" + existedFile.PublicId;

            var isDeleteCloudinaryFile = await DeleteFileCloudinary(cloudPublicId);
            if (isDeleteCloudinaryFile == true)
            {

                if (!_cloudFileRepository.DeleteFile(fileID))
                {
                    ModelState.AddModelError("", "Something went wrong deleting file");
                }
                else
                {
                    return Ok("Cloud File has been removed successfully in DB and Cloudinary!");
                }
            }

            return NoContent();
        }

        #endregion

        #region Cloud CloudFile

        private async Task<string> UploadFileCloudinary(CreateCloudinaryFileDto input)
        {
            var cloudinaryFormat = CloudinaryFormatMapper.MapToCloudinaryFormat(input.Format);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(input.ImagePath),
                PublicId = input.PublicId,
                Overwrite = input.IsOverride,
                Faces = true,
                Folder = input.FolderName,
                Format = cloudinaryFormat
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK || uploadResult.StatusCode == HttpStatusCode.Created)
            {
                return uploadResult.Url.ToString();
            }
            else
            {
                throw new Exception($"Cloudinary upload failed with status code: {uploadResult.StatusCode}");
            }
        }

        private async Task<string> UpdateFileCloudinary(UpdateCloudinaryFileDto input)
        {
            var isExisted = CheckExistedFile(input.PublicId);
            if (!isExisted)
                throw new NotFoundException($"Cloudinary File with public ID '{input.PublicId}' not found.");

            var cloudinaryFormat = CloudinaryFormatMapper.MapToCloudinaryFormat(input.Format);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(input.ImagePath),
                PublicId = input.PublicId,
                Overwrite = input.IsOverride,
                Faces = true,
                Format = cloudinaryFormat
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK || uploadResult.StatusCode == HttpStatusCode.Created)
            {
                return uploadResult.Url.ToString();
            }
            else
            {
                throw new Exception($"Cloudinary upload failed with status code: {uploadResult.StatusCode}");
            }
        }

        private async Task<bool> DeleteFileCloudinary(string publicId)
        {
            var existed = CheckExistedFile(publicId);
            if( existed == false )
                throw new NotFoundException($"Cloudinary File with public ID '{publicId}' not found.");

            DeletionParams deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image,
                Type = "upload"
            };

            DeletionResult deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "ok")
                return true;
            else
                return false;
        }

        private bool CheckExistedFile(string publicId)
        {
            if(publicId == null)
                throw new BadRequestException("Public ID cannot be null.");

            SearchResult result = _cloudinary.Search()
                .Expression($"public_id={publicId}")
                .Execute();

            if (result.Resources != null && result.Resources.Count > 0)
                return true;
            else 
                return false;
        }

        private bool CheckFileExistedInFolder(string publicId, string folder)
        {
            string assetPublicId = $"{folder}/{publicId}";

            SearchResult result = _cloudinary.Search()
               .Expression($"folder={folder} AND public_id={assetPublicId}")
               .Execute();

            if (result.Resources != null && result.Resources.Count > 0)
                return true;
            else 
                return false;
        }

        #endregion
    }
}
