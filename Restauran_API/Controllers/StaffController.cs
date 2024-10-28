using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;
using System.Data;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        QLNhaHangContext dbc;
        public StaffController(QLNhaHangContext db)
        {
            dbc = db;
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
            var existingStaff = dbc.staff.FirstOrDefault(m => m.StaffId == newStaff.StaffId);
            if (existingStaff != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Nhân viên đã tồn tại." });
            }

            dbc.staff.Add(newStaff);
            dbc.SaveChanges();

            return Ok(new { data = dbc.staff.ToList() });
        }



        [HttpPut]
        [Route("/Staff/Update")]
        public IActionResult Sua(int staffId, string username, string password, string name, string role)
        {
            var hh = dbc.staff.FirstOrDefault(c => c.StaffId == staffId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với staffId này." });
            }
                hh.Username = username;
                hh.Password = password;
                hh.FullName = name;
                hh.Role = role;
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
                role = staff.Role
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


    }





}

