using Abp.Collections.Extensions;
using Abp.UI;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryTest.Common;
using CloudinaryTest.Common.Manager;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using CloudinaryTest.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CloudinaryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudFolderAdvanceController : Controller
    {
        private readonly ICloudFolderRepository _cloudFolderRepository;
        private readonly IMapper _mapper;
        private readonly CommonManager _commonManager;

        private readonly Cloudinary _cloudinary;

        public const string CLOUD_NAME = "dduv8pom4";
        public const string API_KEY = "952444439587681";
        public const string API_SECRET = "ubB0ir_v5YXR4KxmnZnuQHORoew";

        public CloudFolderAdvanceController(
            ICloudFolderRepository cloudFolderRepository,
            IMapper mapper,
            CommonManager commonManager
            )
        {
            _cloudFolderRepository = cloudFolderRepository;
            _mapper = mapper;
            _commonManager = commonManager;

            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            _cloudinary = new Cloudinary(account);
        }

        #region LocalDB CloudFolder

        [HttpPost("GetAll")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetAll([FromBody] InputToGetFolderDto input)
        {
            try
            {
                var listFCs = _cloudFolderRepository.GetAll().ToList();
                var listData = listFCs
                    .Select(x => new CloudFolderDto
                    {
                        ID = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Level = x.Level,
                        IsLeaf = x.IsLeaf,
                        ParentId = x.ParentId,
                        Code = x.Code,
                        CombineName = x.CombineName,
                    })
                    .OrderBy(x => CommonUtil.GetNaturalSortKey(x.Code))
                    .ToList();

                if (input.IsGetAll())
                {
                    var treeFolderDto = new TreeFolderDto
                    {
                        Childrens = listData.GenerateTree(c => c.ID, c => c.ParentId)
                    };
                    return Ok(treeFolderDto);
                }

                var listFolderIds = listData
                    .WhereIf(input.IsLeaf.HasValue, x => x.IsLeaf == input.IsLeaf)
                    .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                    .WhereIf(!string.IsNullOrEmpty(input.SearchText),
                    (x => x.Name.ToLower().Contains(input.SearchText.Trim().ToLower()) ||
                    (x.Code.ToLower().Contains(input.SearchText.Trim().ToLower()))))
                    .Select(x => x.ID)
                    .ToList();

                var resultIds = new List<long>();
                foreach (var id in listFolderIds)
                {
                    resultIds.AddRange(_commonManager.GetAllNodeAndLeafIdById(id, listFCs, true));
                }
                resultIds = resultIds.Distinct().ToList();

                var filteredTreeFolderDto = new TreeFolderDto
                {
                    Childrens = listData
                        .Where(x => resultIds.Contains(x.ID))
                        .ToList()
                        .GenerateTree(c => c.ID, c => c.ParentId),
                };

                return Ok(filteredTreeFolderDto);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing the request.");
            }
        }

        #endregion

        #region LocalDB-Cloud CloudFolder

        [HttpGet("GetFolder/{folderId}")]
        [ProducesResponseType(200, Type = typeof(CloudFolder))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetFolder(long folderId)
        {
            if (!_cloudFolderRepository.FolderExists(folderId))
                return NotFound();

            var folder = _mapper.Map<CloudFolderDto>(_cloudFolderRepository.GetFolder(folderId));

            var isCloudExisted = await CheckExistedFolder(folder.CombineName);
            if (!isCloudExisted)
            {
                return BadRequest("This folder exists in DB but it does not existed in Cloudinary!");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(folder);
        }

        [HttpPost("CreateFolder")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateFolder(CreateCloudFolderDto input)
        {
            var existCF = _cloudFolderRepository.GetAll().Any(x => x.Name == input.Name || x.Code == input.Code);
            if (existCF)
            {
                throw new UserFriendlyException(String.Format("Name or Code already exist in Cloud Folders!"));
            }
            if (string.IsNullOrEmpty(input.Name.Trim()))
            {
                throw new UserFriendlyException(String.Format("Name can't be Null or Empty!"));
            }
            var folderMap = _mapper.Map<CloudFolder>(input);
            folderMap.IsActive = true;
            folderMap.IsLeaf = true;

            if (input.ParentId.HasValue)
            {
                var parent = _cloudFolderRepository.GetFolder(input.ParentId.Value);
                parent.IsLeaf = false;

                folderMap.Level = parent.Level + 1;
                folderMap.CombineName = parent.CombineName + "/" + folderMap.Name;

                // Save changes after modifying entities
                _cloudFolderRepository.UpdateFolder(parent);  // Assuming you have an UpdateFolder method
            }
            else
            {
                folderMap.Level = 1;
                folderMap.CombineName = folderMap.Name;
            }

            var isCloudExisted = await CheckExistedFolder(folderMap.CombineName);
            if (isCloudExisted)
            {
                return BadRequest("A folder with that name already exists in Cloudinary!");
            }
            else
            {
                await _cloudinary.CreateFolderAsync(folderMap.CombineName);
            }

            if (!_cloudFolderRepository.CreateFolder(folderMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Cloud Folder created successfully in DB and Cloudinary!");
        }

        [HttpPut("UpdateFolder")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFolder(UpdateCloudFolderDto input)
        {
            var cloudFolder = _cloudFolderRepository.GetFolder(input.ID);
            if (cloudFolder == default) { throw new UserFriendlyException($"Can not found folder with Id = {input.ID}"); }

            var duplicateFolder = _cloudFolderRepository.GetAll().Any(x => (x.Name == input.Name || x.Code == input.Code) && x.Id != input.ID);

            if (duplicateFolder)
            {
                throw new UserFriendlyException(String.Format("Name or Code already exist in Cloud Folder!"));
            }

            if (string.IsNullOrEmpty(input.Name.Trim()))
            {
                throw new UserFriendlyException(String.Format("Name of Cloud Folder can't be Null or Empty!"));
            }

            if (input.Code != cloudFolder.Code && !cloudFolder.IsLeaf)
            {
                RenameChildCode(input);
            }

            var oldCloudName = cloudFolder.CombineName;

            var split = cloudFolder.CombineName.Split('/');
            var countSlash = split.Length - 1;
            if (input.Name != cloudFolder.Name)
            {
                var splitCombineName = cloudFolder.CombineName.Split('/');
                splitCombineName[countSlash] = input.Name;
                cloudFolder.CombineName = string.Join('/', splitCombineName);

                //await UpdateCloudinaryFolder(oldCloudName, cloudFolder.CombineName);

                _cloudFolderRepository.UpdateFolder(cloudFolder);

                await RenameChildCombineName(input);
            }

            var folerMap = _mapper.Map<CloudFolder>(input);
            if (!_cloudFolderRepository.UpdateFolder(folerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private void RenameChildCode(UpdateCloudFolderDto input)
        {
            var listCFs = _cloudFolderRepository.GetAll().ToList();
            var listChildIds = _commonManager.GetAllChildId(input.ID, listCFs).Distinct();
            var listChilds = listCFs.Where(x => listChildIds.Contains(x.Id) && x.Id != input.ID).ToList();

            var split = input.Code.Split('.');
            var countDot = split.Length - 1;
            for (int i = 0; i < listChilds.Count; i++)
            {
                var splitChild = listChilds[i].Code.Split(".");
                splitChild[countDot] = split[countDot];
                listChilds[i].Code = string.Join(".", splitChild);
            }

            _cloudFolderRepository.UpdateRange(listChilds);
        }

        private async Task RenameChildCombineName(UpdateCloudFolderDto input)
        {
            var cloudFolder = _cloudFolderRepository.GetFolder(input.ID);
            var listCFs = _cloudFolderRepository.GetAll().ToList();
            var listChildIds = _commonManager.GetAllChildId(input.ID, listCFs).Distinct();
            var listChilds = listCFs.Where(x => listChildIds.Contains(x.Id) && x.Id != input.ID).ToList();

            var split = cloudFolder.CombineName.Split('/');
            var countSlash = split.Length - 1;
            for (int i = 0; i < listChilds.Count; i++)
            {
                var oldCloudName = listChilds[i].CombineName;
                var splitChild = listChilds[i].CombineName.Split('/');
                splitChild[countSlash] = split[countSlash].Trim();
                listChilds[i].CombineName = string.Join("/", splitChild);
                //await UpdateCloudinaryFolder(oldCloudName, listChilds[i].CombineName);
            }

            _cloudFolderRepository.UpdateRange(listChilds);
        }

        [HttpDelete("DeleteFolder")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFolder(long id)
        {
            var folder = await _cloudFolderRepository.GetFolderAsync(id);
            if (folder == default)
                throw new UserFriendlyException($"Can not found Folder with Id = {id}");

            long? parentID = folder.ParentId;

            var allFolders = _cloudFolderRepository.GetAll().OrderBy(x => x.Name).ToList();

            if (!folder.IsLeaf)
            {
                var listIds = _commonManager.GetAllNodeAndLeafIdById(id, allFolders).Distinct().ToList();
                var deleteTasks = listIds
                    .Select(async Id =>
                    {
                        ValidToDeleteSubFolder(Id);
                        var childFolder = allFolders.FirstOrDefault(f => f.Id == Id);
                        if (childFolder != null)
                        {
                            var isDelInCloud = await DeleteCloudinaryFolder(childFolder.CombineName);
                            if (isDelInCloud)
                                await _cloudFolderRepository.DeleteFolderAsync(Id);
                        }
                    });

                await Task.WhenAll(deleteTasks);
            }
            else
            {
                ValidToDeleteSubFolder(id);
                var isDelInCloud = await DeleteCloudinaryFolder(folder.CombineName);
                if (isDelInCloud)
                    await _cloudFolderRepository.DeleteFolderAsync(id);
            }

            if (parentID.HasValue)
            {
                var parent = await _cloudFolderRepository.GetFolderAsync(parentID.Value);
                var countRemainChild = _cloudFolderRepository.GetAll().Any(child => child.ParentId == parentID);
                if (!countRemainChild)
                {
                    parent.IsLeaf = true;
                    await _cloudFolderRepository.UpdateFolderAsync(parent);
                }
            }

            return NoContent();
        }


        private void ValidToDeleteSubFolder(long id)
        {
            var folder = _cloudFolderRepository.GetAll()
                .Where(x => x.CloudFiles != null && x.CloudFiles.Count > 0 && x.Id == id)
                .FirstOrDefault();
            if (folder != default)
                throw new UserFriendlyException($"Can not delete folder because it has assets");
        }

        #endregion

        #region Cloud CloudFolder

        private async Task<bool> UpdateCloudinaryFolder(string currentFolderName, string newFolderName)
        {
            var isExisted = await CheckExistedFolder(currentFolderName);
            if(!isExisted)
                throw new Exception($"Folder with name {currentFolderName} does not exist in Cloudinary!");

            var deleteFolder = await _cloudinary.DeleteFolderAsync($"{currentFolderName}");

            if (deleteFolder.StatusCode == HttpStatusCode.OK)
            {
                await _cloudinary.CreateFolderAsync(newFolderName);
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpDelete("DeleteCloudinaryFolder")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<bool> DeleteCloudinaryFolder(string folderName)
        {
            var isExisted = await CheckExistedFolder(folderName);
            if (isExisted)
            {
                var deleteFolder = await _cloudinary.DeleteFolderAsync($"{folderName}");

                if (deleteFolder.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    throw new Exception($"Folder {folderName} might still contains assets inside it in Cloudinary!");
            }
            else
                throw new Exception($"Folder {folderName} does not exist in Cloudinary!");
        }

        private async Task<bool> CheckExistedFolder(string name)
        {
            var subFoldersResult = await _cloudinary.SubFoldersAsync(name);

            if (subFoldersResult != null && subFoldersResult.Folders != null)
                return true;
            else
                return false;
        }

        #endregion

    }
}
