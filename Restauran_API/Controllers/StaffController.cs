using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Restauran_API.Models;
using System.Data;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        QLNhaHangContext dbc;
        private readonly IHostEnvironment _hostEnvironment; // Thêm trường host environment
        public StaffController(QLNhaHangContext db, IHostEnvironment hostEnvironment)
        {
            dbc = db;
            _hostEnvironment = hostEnvironment; // Khởi tạo trường
        }
        [HttpGet]
        [Route("/Staff/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.staff.ToList());
        }
        [HttpPost]
        [Route("/Staff/Delete")]
        public IActionResult DeleteStaff(int id)
        {
            var staff = dbc.staff
                          .Include(t => t.Shifts) // Bao gồm các ca làm việc liên quan
                          .FirstOrDefault(t => t.StaffId == id);

            if (staff == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(staff.Image)) // Kiểm tra xem có hình ảnh không
            {
                DeleteImage(staff.Image);
            }
            dbc.Shifts.RemoveRange(staff.Shifts);


                // Cuối cùng, xóa nhân viên
                dbc.staff.Remove(staff);
                dbc.SaveChanges();

                return Ok(dbc.staff.ToList());
        }

        [HttpPost]
        [Route("/Staff/Insert")]
        public IActionResult Them([FromBody] staff newStaff)
        {
            var existingStaff = dbc.staff.FirstOrDefault(m => m.Username == newStaff.Username && m.Username != "Unavailable");
            if (existingStaff != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Tên tài khoản đã tồn tại." });
            }

            dbc.staff.Add(newStaff);
            dbc.SaveChanges();

            return Ok(new { data = dbc.staff.ToList() });
        }



        [HttpPut]
        [Route("/Staff/Update")]
        public IActionResult Sua([FromBody] staff updateStaff)
        {
            var existingStaff = dbc.staff.FirstOrDefault(m => m.StaffId == updateStaff.StaffId);
            if (existingStaff == null)
            {
                return BadRequest(new { message = "Nhân viên không tồn tại." });
            }

            existingStaff.Username = updateStaff.Username;
            existingStaff.FullName = updateStaff.FullName;
            existingStaff.Password = updateStaff.Password;
            existingStaff.Role = updateStaff.Role;
            existingStaff.Image = updateStaff.Image;


            dbc.SaveChanges();

            return Ok(new { data = dbc.staff.ToList() });
        }
        [HttpPost]
        [Route("/Staff/Login")]
        public IActionResult DangNhap(string username, string password)
        {
            var staff = dbc.staff.FirstOrDefault(s => s.Username == username && s.Password == password);
            if (staff == null)
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });
            }
            return Ok(new
            {
                message = "Đăng nhập thành công",
                staffId = staff.StaffId,
                fullName = staff.FullName, // Include FullName in the response
                role = staff.Role,
                image = staff.Image,
            });
        }
        [HttpGet]
        [Route("/Staff/GetById")]
        public IActionResult GetById(int id)
        {
            var vc = dbc.staff.FirstOrDefault(m => m.StaffId == id);
            if (vc == null)
            {
                return NotFound(new { message = "Không tìm thấy với StaffId này." });
            }

            return Ok(vc);
        }

        private IActionResult DeleteImage(string imageName)
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

