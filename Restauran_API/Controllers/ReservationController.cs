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
        public IActionResult Them([FromBody] Reservation newReservation)
        {
            var existingReservation = dbc.Reservations.FirstOrDefault(m => m.ReservationId == newReservation.ReservationId);
            if (existingReservation != null)
            {
                return BadRequest(new { message = "Tài khoản đã tồn tại." });
            }

            newReservation.Status = "waiting"; // Thiết lập mặc định cho Point
           

            dbc.Reservations.Add(newReservation);
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
        [HttpGet]
        [Route("/Reservation/AvailableTables")]
        public IActionResult GetAvailableTables(DateTime reservationDateTime)
        {
            // Lấy tất cả các bàn có trạng thái "Available"
            var availableTables = dbc.Tables.Where(t => t.Status == "Available").ToList();

            // Lấy các bàn đã đặt ở thời điểm cụ thể
            var reservedTables = dbc.Reservations
                                    .Where(r => r.ReservationTime == reservationDateTime)
                                    .Select(r => r.TableId)
                                    .ToList();

            // Loại bỏ các bàn đã đặt từ danh sách các bàn "Available"
            var filteredTables = availableTables
                                  .Where(t => !reservedTables.Contains(t.TableId))
                                  .ToList();

            return Ok(filteredTables);
        }
        [HttpGet]
        [Route("/Reservation/ActiveReservations")]
        public IActionResult GetActiveReservations()
        {
            var currentTime = DateTime.Now;
            var currentDate = currentTime.Date; // Lấy ngày hiện tại mà không có giờ

            // Lọc các Reservation có thời gian lớn hơn hiện tại và cùng ngày
            var activeReservations = dbc.Reservations
                .Where(r => r.ReservationTime.HasValue && r.ReservationTime.Value > currentTime && r.ReservationTime.Value.Date == currentDate)
                .ToList();

            // Lọc các Reservation có thời gian bé hơn hiện tại và cập nhật trạng thái thành "overtime"
            var overdueReservations = dbc.Reservations
                .Where(r => r.ReservationTime.HasValue && r.ReservationTime.Value <= currentTime && r.Status != "overtime")
                .ToList();

            foreach (var reservation in overdueReservations)
            {
                reservation.Status = "overtime"; // Cập nhật trạng thái thành "overtime"
            }

            dbc.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            return Ok(new
            {
                activeReservations = activeReservations, // Trả về các Reservation còn hiệu lực
                updatedReservations = overdueReservations // Trả về các Reservation đã được cập nhật trạng thái
            });
        }


    }
}
