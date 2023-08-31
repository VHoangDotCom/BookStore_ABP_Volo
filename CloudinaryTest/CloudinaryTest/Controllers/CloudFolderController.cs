using Abp.Collections.Extensions;
using Abp.UI;
using AutoMapper;
using CloudinaryTest.Common;
using CloudinaryTest.Common.Manager;
using CloudinaryTest.CoreHelper;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using CloudinaryTest.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;

namespace CloudinaryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudFolderController : Controller
    {
        private readonly ICloudFolderRepository _cloudFolderRepository;
        private readonly IMapper _mapper;
        private readonly CommonManager _commonManager;
        // private readonly IWorkScope _workScope;

        public CloudFolderController(
            ICloudFolderRepository cloudFolderRepository,
            IMapper mapper,
            CommonManager commonManager
           // IWorkScope workScope
            )
        {
            _cloudFolderRepository = cloudFolderRepository;
            _mapper = mapper;
            _commonManager = commonManager;
           // _workScope = workScope;
        }

        /*[HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CloudFolder>))]
        public IActionResult GetCloudFolders()
        {
            var cloudFolders = _mapper.Map<List<CloudFolderDto>>(_cloudFolderRepository.GetAll());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(cloudFolders);
        }*/

        /* [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCloudFolder([FromBody] CloudFolderDto cloudFolder)
        {
            if (cloudFolder == null) return BadRequest(ModelState);

            var cloudFolders = _cloudFolderRepository.GetFolderTrimToUpper(cloudFolder);

            if(cloudFolders != null)
            {
                ModelState.AddModelError("", "Cloud Folder already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var folderMap = _mapper.Map<CloudFolder>(cloudFolder);

            if(!_cloudFolderRepository.CreateFolder(folderMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }*/


        /*   [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCloudFolder([FromBody] CloudFolderDto cloudFolder)
        {
            if (cloudFolder == null)
                return BadRequest(ModelState);

            if (!_cloudFolderRepository.FolderExists(cloudFolder.ID))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var folerMap = _mapper.Map<CloudFolder>(cloudFolder);

            if (!_cloudFolderRepository.UpdateFolder(folerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }*/

        [HttpPost("/api/CloudFolderTree")]
        [ProducesResponseType(200)]  // Ok
        [ProducesResponseType(400)]  // Bad Request
        public IActionResult GetCloudFoldersTree([FromBody] InputToGetFolderDto input)
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

        [HttpGet("/api/CloudFolderTree")]
        [ProducesResponseType(200)]  // Ok
        [ProducesResponseType(400)]  // Bad Request
        public IActionResult GetCloudFoldersTree()
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

                var treeFolderDto = new TreeFolderDto
                {
                    Childrens = listData.GenerateTree(c => c.ID, c => c.ParentId)
                };

                return Ok(treeFolderDto);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing the request.");
            }
        }

        [HttpGet("{folderId}")]
        [ProducesResponseType(200, Type = typeof(CloudFolder))]
        [ProducesResponseType(400)]
        public IActionResult GetFolder(long folderId)
        {
            if (!_cloudFolderRepository.FolderExists(folderId))
                return NotFound();

            var fodler = _mapper.Map<CloudFolderDto>(_cloudFolderRepository.GetFolder(folderId));

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(fodler);
        }

        [HttpPost("/api/CreateTreeCloudFolder")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTreeCloudFolder(CreateCloudFolderDto input)
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

            if (!_cloudFolderRepository.CreateFolder(folderMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("/api/UpdateTreeCloudFolder")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTreeCloudFolder(UpdateCloudFolderDto input)
        {
            var cloudFolder = _cloudFolderRepository.GetFolder(input.ID);
            if (cloudFolder == default) { throw new UserFriendlyException($"Can not found folder with Id = {input.ID}"); }

            var exist = _cloudFolderRepository.GetAll().Any(x => (x.Name == input.Name || x.Code == input.Code) && x.Id != input.ID);

            if (exist)
            {
                throw new UserFriendlyException(String.Format("Name or Code already exist in Cloud Folder!"));
            }

            if (string.IsNullOrEmpty(input.Name.Trim()))
            {
                throw new UserFriendlyException(String.Format("Name of Cloud Folder can't be Null or Empty!"));
            }

            if(input.Code != cloudFolder.Code && !cloudFolder.IsLeaf)
            {
                 RenameChildCode(input);
            }

            var split = cloudFolder.CombineName.Split('/');
            var countSlash = split.Length - 1;
            if(input.Name != cloudFolder.Name )
            {
                var splitCombineName = cloudFolder.CombineName.Split('/');
                splitCombineName[countSlash] = input.Name;
                cloudFolder.CombineName = string.Join('/', splitCombineName);

                _cloudFolderRepository.UpdateFolder(cloudFolder);
                RenameChildCombineName(input);
            }

            var folerMap = _mapper.Map<CloudFolder>(input);
            if (!_cloudFolderRepository.UpdateFolder(folerMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private void RenameChildCombineName(UpdateCloudFolderDto input)
        {
            var cloudFolder = _cloudFolderRepository.GetFolder(input.ID);
            var listCFs = _cloudFolderRepository.GetAll().ToList();
            var listChildIds = _commonManager.GetAllChildId(input.ID, listCFs).Distinct();
            var listChilds = listCFs.Where(x => listChildIds.Contains(x.Id) && x.Id != input.ID).ToList();

            var split = cloudFolder.CombineName.Split('/');
            var countSlash = split.Length - 1;
            for(int i = 0; i < listChilds.Count; i++)
            {
                var splitChild = listChilds[i].CombineName.Split('/');
                splitChild[countSlash] = split[countSlash].Trim();
                listChilds[i].CombineName = string.Join("/", splitChild);
            }

            _cloudFolderRepository.UpdateRange(listChilds);
        }

        private void RenameChildCode(UpdateCloudFolderDto input)
        {
            var listCFs = _cloudFolderRepository.GetAll().ToList();
            var listChildIds = _commonManager.GetAllChildId(input.ID, listCFs).Distinct();
            var listChilds = listCFs.Where(x => listChildIds.Contains(x.Id) && x.Id != input.ID).ToList();

            var split = input.Code.Split('.');
            var countDot = split.Length - 1;
            for(int i = 0; i < listChilds.Count; i++)
            {
                var splitChild = listChilds[i].Code.Split(".");
                splitChild[countDot] = split[countDot];
                listChilds[i].Code = string.Join(".", splitChild);
            }

            _cloudFolderRepository.UpdateRange(listChilds);
        }

        [HttpDelete("/api/DeleteTreeCloudFolder")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTreeCloudFolder(long id)
        {
            var folder = _cloudFolderRepository.GetFolder(id);
            if(folder == default)
                throw new UserFriendlyException($"Can not found Folder with Id = {id}");

            if (!folder.IsLeaf)
            {
                var listFCs = _cloudFolderRepository.GetAll().ToList();
                var listIds = _commonManager.GetAllNodeAndLeafIdById(id, listFCs).Distinct().ToList();
                foreach(var Id in listIds)
                {
                    ValidToDeleteSubFolder(Id);
                    _cloudFolderRepository.DeleteFolder(Id);
                }
            }
            else
            {
                ValidToDeleteSubFolder(id);
                _cloudFolderRepository.DeleteFolder(id);
            }

            if(folder.ParentId.HasValue)
            {
                var parentID = folder.ParentId.Value;
                var parent = _cloudFolderRepository.GetFolder(parentID);
                var countRemainChild = _cloudFolderRepository.GetAll().Any(child => child.ParentId == parentID);
                if (!countRemainChild)
                {
                    parent.IsLeaf = true;
                    _cloudFolderRepository.UpdateFolder(parent);
                }
            }

            return NoContent();
        }

        private void ValidToDeleteSubFolder(long id)
        {
            var folder = _cloudFolderRepository.GetAll()
                .Where(x => x.CloudFiles != null && x.CloudFiles.Count > 0 && x.Id == id)
                .FirstOrDefault();
            if(folder != default)
                throw new UserFriendlyException($"Can not delete folder because it has assets");
        }

        private void ValidToDeleteListFolder(long id, List<long> listFolderIds = default)
        {
           //Execute case that its leaf has list Cloud File
        }

        [HttpDelete("{folderId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCloudFolder(long folderId)
        {
            if (!_cloudFolderRepository.FolderExists(folderId))
            {
                return NotFound();
            }

            var folderToDelete = _cloudFolderRepository.GetFolder(folderId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if ((!_cloudFolderRepository.DeleteFolder(folderToDelete.Id)))
            {
                ModelState.AddModelError("", "Something went wrong deleting folder");
            }

            return NoContent();
        }

    }
}
