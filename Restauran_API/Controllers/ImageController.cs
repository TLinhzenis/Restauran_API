using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ImageController(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Không có hình ảnh nào được tải lên.");

            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uploads");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imagePath = $"/uploads/{file.FileName}"; // Đường dẫn tương đối
            return Ok(new { imagePath });
        }

        [HttpGet("list-images")]
        public IActionResult ListImages()
        {
            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uploads");
            var imageFiles = Directory.GetFiles(uploadsFolder).Select(Path.GetFileName).ToList();
            var imagePaths = imageFiles.Select(file => $"/uploads/{file}").ToList();
            return Ok(imagePaths);
        }
        [HttpPost("delete-image")]
        public IActionResult DeleteImage(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return BadRequest("Tên hình ảnh không hợp lệ.");

            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uploads");
            var filePath = Path.Combine(uploadsFolder, imageName);

            // Kiểm tra xem file có tồn tại không
            if (!System.IO.File.Exists(filePath))
                return NotFound("Hình ảnh không tồn tại.");

            // Xóa file hình ảnh
            System.IO.File.Delete(filePath);
            return Ok("Hình ảnh đã được xóa thành công.");
        }

    }
}
