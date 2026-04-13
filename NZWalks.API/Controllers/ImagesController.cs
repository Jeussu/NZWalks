using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }
        // POST: api/images/upload
        [HttpPost]
        [Route("Upload")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);
            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(request.File!.FileName).ToLowerInvariant();

                // convert DTO to domain model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = fileExtension,
                    FileSizeInBytes = request.File.Length,
                    FileName = Guid.NewGuid().ToString("N"),
                    FileDescription = request.FileDescription,
                };

                // User repository to upload image 
                imageDomainModel = await imageRepository.Upload(imageDomainModel);

                return Ok(new ImageDto
                {
                    Id = imageDomainModel.Id,
                    FileName = imageDomainModel.FileName,
                    FileDescription = imageDomainModel.FileDescription,
                    FileExtension = imageDomainModel.FileExtension,
                    FileSizeInBytes = imageDomainModel.FileSizeInBytes,
                    FilePath = imageDomainModel.FilePath
                });
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            if (request.File == null)
            {
                ModelState.AddModelError("File", "File is required.");
                return;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("File", "Unsupported file extension.");
            }

            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("File", "File size more than 10MB, please upload a smaller size file.");
            }
        }
    }
}
