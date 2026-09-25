using MediaSite_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaSite_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController(StorageService storageService) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedTypes.Contains(extension))
                return BadRequest("Invalid file type.");

            var key = $"uploads/{Guid.NewGuid()}{extension}";

            await using var stream = file.OpenReadStream();

            await storageService.UploadAsync(key, stream);

            return Ok(new
            {
                message = "File uploaded successfully.",
                key
            });
        }
    }
}
