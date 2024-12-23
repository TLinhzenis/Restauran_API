using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        QLNhaHangContext dbc;
        public ShiftController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Shift/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Shifts.ToList());
        }
        [HttpPost]
        [Route("/Shift/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.Shifts.FirstOrDefault(v => v.ShiftId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.Shifts.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.Shifts.ToList());
        }
        [HttpPost]
        [Route("/Shift/Insert")]
        public IActionResult Them(int staffid, DateTime starttime)
        {
            Shift hh = new Shift
            {
                StaffId = staffid,
                StartTime = starttime,
            };

            dbc.Shifts.Add(hh);
            dbc.SaveChanges();

            return Ok(hh);
        }



        [HttpPut]
        [Route("/Shift/UpdateEndTime")]
        public IActionResult Sua(int ShiftId, DateTime endtime)
        {
            var hh = dbc.Shifts.FirstOrDefault(c => c.ShiftId == ShiftId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với ShiftId này." });
            }
            hh.EndTime = endtime;
            dbc.SaveChanges();

            return Ok(hh);
        }
        [HttpGet]
        [Route("/Shift/MonthlyData")]
        public IActionResult GetMonthlyData(int month)
        {
            // Lọc danh sách các ca làm việc với StartTime hợp lệ và thuộc tháng cần tìm
            var shifts = dbc.Shifts
                .Where(s => s.StartTime.HasValue && s.StartTime.Value.Month == month)
                .ToList();

            // Lấy danh sách nhân viên
            var staffList = dbc.staff.ToList();

            // Tính toán số ngày làm việc và lương cho từng nhân viên
            var result = staffList.Select(staff => new
            {
                StaffId = staff.StaffId,
                FullName = staff.FullName,
                WorkDays = shifts
                    .Where(s => s.StaffId == staff.StaffId)
                    .Select(s => s.StartTime.Value.Day) // Dùng Value để truy cập DateTime
                    .Distinct()
                    .Count(),
                Salary = shifts
                    .Where(s => s.StaffId == staff.StaffId)
                    .Select(s => s.StartTime.Value.Day) // Dùng Value để truy cập DateTime
                    .Distinct()
                    .Count() * 300000 // Mỗi ngày làm việc được tính 300000
            });

            return Ok(result);
        }
        [HttpGet]
        [Route("/Shift/WorkDays")]
        public IActionResult GetWorkDays(int staffId, int month)
        {
            // Lấy danh sách ngày làm việc của nhân viên
            var workDays = dbc.Shifts
                .Where(s => s.StaffId == staffId && s.StartTime.HasValue && s.StartTime.Value.Month == month)
                .Select(s => s.StartTime.Value.Day)
                .Distinct()
                .ToList();

            // Lấy tên nhân viên tương ứng với staffId
            var staffName = dbc.staff
                .Where(staff => staff.StaffId == staffId)
                .Select(staff => staff.FullName) // Giả sử trường tên nhân viên là FullName
                .FirstOrDefault();

            // Kiểm tra nếu không tìm thấy nhân viên
            if (staffName == null)
            {
                return NotFound("Nhân viên không tồn tại.");
            }

            // Trả về kết quả bao gồm cả tên nhân viên và danh sách ngày làm việc
            var result = new
            {
                StaffName = staffName,
                WorkDays = workDays
            };

            return Ok(result);
        }


    }
}
