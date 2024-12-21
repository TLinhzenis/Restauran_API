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
    }
}
