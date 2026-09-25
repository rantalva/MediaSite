using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MediaSite_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedTypes = new[] { ".jpg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedTypes.Contains(extension))
                return BadRequest("Invalid file type.");

            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var uploadsFolder = Path.Combine(wwwroot, "Uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "File uploaded successfully." });
        }
    }
}
