using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        QLNhaHangContext dbc;
        public ReservationController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Reservation/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Reservations.ToList());
        }
        [HttpPost]
        [Route("/Reservation/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.Reservations.FirstOrDefault(v => v.ReservationId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.Reservations.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.Reservations.ToList());
        }
        [HttpPost]
        [Route("/Reservation/Insert")]
        public IActionResult Them(int customerid, int tableid, string status, DateTime ReservationTime)
        {
            Reservation hh = new Reservation
            {
                CustomerId = customerid,
                TableId = tableid,
                ReservationTime = ReservationTime,
                Status = status

            };

            dbc.Reservations.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Reservations.ToList() });
        }



        [HttpPut]
        [Route("/Reservation/Update")]
        public IActionResult Sua(int ReservationId, int customerid, int tableid, string status, DateTime ReservationTime)
        {
            var hh = dbc.Reservations.FirstOrDefault(c => c.ReservationId == ReservationId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với ReservationId này." });
            }
                hh.CustomerId = customerid;
                hh.TableId = tableid;
                hh.ReservationTime = ReservationTime;
                hh.Status = status;
            dbc.SaveChanges();

            return Ok(new { data = dbc.Reservations.ToList() });
        }
    }
}
