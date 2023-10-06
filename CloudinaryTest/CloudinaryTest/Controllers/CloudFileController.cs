using AutoMapper;
using CloudinaryTest.Dto;
using CloudinaryTest.Entities;
using CloudinaryTest.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudinaryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudFileController : Controller
    {
        private readonly ICloudFileRepository _cloudFileRepository;
        private readonly ICloudFolderRepository _cloudFolderRepository;
        private readonly IMapper _mapper;

        public CloudFileController(
            ICloudFileRepository repository,
            ICloudFolderRepository cloudFolderRepository,
            IMapper mapper
            )
        {
            _cloudFileRepository = repository;
            _cloudFolderRepository = cloudFolderRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GetCloudFileDto>))]
        public IActionResult GetCloudFiles()
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

        [HttpGet("{fileID}")]
        [ProducesResponseType(200, Type = typeof(CloudFile))]
        [ProducesResponseType(400)]
        public IActionResult GetCloudFile(long fileID)
        {
            if (!_cloudFileRepository.CloudFileExists(fileID))
                return NotFound();

            var category = _mapper.Map<GetCloudFileDto>(_cloudFileRepository.GetFile(fileID));

            category.CloudFolder = _mapper.Map<CloudFolderDto>(_cloudFolderRepository.GetFolder(category.FolderId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateFile([FromBody] CreateCloudFileDto fileCreate)
        {
            if (fileCreate == null)
                return BadRequest(ModelState);

            var fileExisted = _cloudFileRepository.GetAll()
                .Where(c => c.PublicId.Trim().ToUpper() == fileCreate.PublicId.TrimEnd().ToUpper())
                .FirstOrDefault();

            var folderExisted = _cloudFolderRepository.GetFolder(fileCreate.FolderId);

            if (fileExisted != null)
            {
                ModelState.AddModelError("", "File already exists");
                return StatusCode(422, ModelState);
            }

            if (folderExisted == null)
            {
                ModelState.AddModelError("", "Folder does not exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fileMap = _mapper.Map<CloudFile>(fileCreate);

            fileMap.FolderPath = folderExisted.CombineName;

            if (!_cloudFileRepository.CreateFile(fileMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFile([FromBody] UpdateCloudFileDto updatedFile)
        {
            if (updatedFile == null)
                return BadRequest(ModelState);

            var existingFile = _cloudFileRepository.GetFile(updatedFile.Id);

            if (existingFile == null)
                return NotFound();

           // existingFile.PublicId = updatedFile.PublicId;
            existingFile.Format = updatedFile.Format;
            //existingFile.ImageURL = updatedFile.ImageURL;
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

        [HttpDelete("{fileID}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteFile(long fileID)
        {
            if (!_cloudFileRepository.CloudFileExists(fileID))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_cloudFileRepository.DeleteFile(fileID))
            {
                ModelState.AddModelError("", "Something went wrong deleting file");
            }

            return NoContent();
        }

    }
}
