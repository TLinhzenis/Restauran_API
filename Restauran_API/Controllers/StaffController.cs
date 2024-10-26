using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;
using System.Data;

namespace Restauran_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        QLNhaHangContext dbc;
        public StaffController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/staff/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.staff.ToList());
        }
        [HttpPost]
        [Route("/staff/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.staff.FirstOrDefault(v => v.StaffId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.staff.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.staff.ToList());
        }
        [HttpPost]
        [Route("/staff/Insert")]
        public IActionResult Them(string username, string password, string name, string role)
        {
            staff hh = new staff
            {
                Username = username,
                Password = password,
                FullName = name,
                Role = role

            };

            dbc.staff.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.staff.ToList() });
        }



        [HttpPut]
        [Route("/staff/Update")]
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
        [Route("/staff/Login")]
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
                fullName = staff.FullName // Include FullName in the response
            });
        }




    }
}
