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
        private readonly IMapper _mapper;

        public CloudFileController(ICloudFileRepository repository, IMapper mapper)
        {
            _cloudFileRepository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CloudFileDto>))]
        public IActionResult GetCloudFiles()
        {
            var files = _mapper.Map<List<CloudFileDto>>(_cloudFileRepository.GetAll());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(files);
        }

        [HttpGet("{fileID}")]
        [ProducesResponseType(200, Type = typeof(CloudFile))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(long fileID)
        {
            if (!_cloudFileRepository.CloudFileExists(fileID))
                return NotFound();

            var category = _mapper.Map<CloudFileDto>(_cloudFileRepository.GetFile(fileID));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateFile([FromBody] CloudFileDto fileCreate)
        {
            if (fileCreate == null)
                return BadRequest(ModelState);

            var category = _cloudFileRepository.GetAll()
                .Where(c => c.PublicId.Trim().ToUpper() == fileCreate.PublicId.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "File already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fileMap = _mapper.Map<CloudFile>(fileCreate);

            if (!_cloudFileRepository.CreateFile(fileMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{fileId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateFile(long fileId, [FromBody] CloudFileDto updatedFile)
        {
            if (updatedFile == null)
                return BadRequest(ModelState);

            if (fileId != updatedFile.Id)
                return BadRequest(ModelState);

            if (!_cloudFileRepository.CloudFileExists(fileId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var fileMap = _mapper.Map<CloudFile>(updatedFile);

            if (!_cloudFileRepository.UpdateFile(fileMap))
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
