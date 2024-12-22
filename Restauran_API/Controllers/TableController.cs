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
        public IActionResult Sua([FromBody] Table updateTable)
        {
            var existingTable = dbc.Tables.FirstOrDefault(m => m.TableId == updateTable.TableId);
            if (existingTable == null)
            {
                return BadRequest(new { message = "Table không tồn tại." });
            }

            existingTable.TableNumber = updateTable.TableNumber;
            existingTable.Capacity = updateTable.Capacity;
            existingTable.Status = updateTable.Status;
            existingTable.Username = updateTable.Username;
            existingTable.Password = updateTable.Password;

            dbc.SaveChanges();

            return Ok(new { data = dbc.Tables.ToList() });
        }

        [HttpPut]
        [Route("/Table/UpdateStatus")]
        public IActionResult Sua(int tableID, string status)
        {
            var existingTable = dbc.Tables.FirstOrDefault(m => m.TableId == tableID);
            if (existingTable == null)
            {
                return BadRequest(new { message = "Table không tồn tại." });
            }

            existingTable.Status = status;


            dbc.SaveChanges();

            return Ok(existingTable);
        }
        [HttpGet]
        [Route("/Table/GetById")]
        public IActionResult GetById(int id)
        {
            var vc = dbc.Tables.FirstOrDefault(m => m.TableId == id);
            if (vc == null)
            {
                return NotFound(new { message = "Không tìm thấy với TableId này." });
            }

            return Ok(vc);
        }
    }
}
