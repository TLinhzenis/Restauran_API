using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class TableController : ControllerBase
    {
        QLNhaHangContext dbc;
        public TableController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Table/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Tables.ToList());
        }
        [HttpPost]
        [Route("/Table/Delete")]
        public IActionResult Xoa(int id)
        {
            var table = dbc.Tables
                           .Include(t => t.Orders)        // Bao gồm các đơn hàng liên quan
                           .Include(t => t.Reservations)  // Bao gồm các đặt bàn liên quan
                           .FirstOrDefault(t => t.TableId == id);

            if (table == null)
            {
                return NotFound();
            }

            // Xóa tất cả các bản ghi Orders và Reservations liên quan
            dbc.Orders.RemoveRange(table.Orders);
            dbc.Reservations.RemoveRange(table.Reservations);

            // Cuối cùng, xóa bàn
            dbc.Tables.Remove(table);

            dbc.SaveChanges();

            return Ok(dbc.Tables.ToList());
        }

        [HttpPost]
        [Route("/Table/Insert")]
        public IActionResult Them(string tableNumber, string status, int capacity, string username, string password)
        {
            Table hh = new Table
            {
                TableNumber = tableNumber,
                Status = status,
                Capacity = capacity,
                Username = username, 
                Password = password

            };

            dbc.Tables.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Tables.ToList() });
        }



        [HttpPut]
        [Route("/Table/Update")]
        public IActionResult Sua(int TableId, string tableNumber, string status, int capacity, string username, string password)
        {
            var hh = dbc.Tables.FirstOrDefault(c => c.TableId == TableId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với TableId này." });
            }
                hh.TableNumber = tableNumber;
                hh.Status = status;
                hh.Capacity = capacity;
                hh.Username = username;
                hh.Password = password;
            dbc.SaveChanges();

            return Ok(new { data = dbc.Tables.ToList() });
        }
    }
}
