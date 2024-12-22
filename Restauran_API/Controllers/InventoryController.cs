using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        QLNhaHangContext dbc;
        public InventoryController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Inventory/List")]
        public IActionResult GetList()
        {
            var inventories = dbc.Inventories
                                 .Include(i => i.Supplier) // Bao gồm thông tin Supplier
                                 .Select(i => new
                                 {
                                     i.ItemId,
                                     i.ItemName,
                                     i.Quantity,
                                     SupplierName = i.Supplier != null ? i.Supplier.SupplierName : "Không xác định" // Trả về tên hoặc thông báo
                                 })
                                 .ToList();

            return Ok(inventories);
        }

        [HttpPost]
        [Route("/Inventory/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.Inventories.FirstOrDefault(v => v.ItemId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.Inventories.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.Inventories.ToList());
        }
        [HttpPost]
        [Route("/Inventory/Insert")]
        public IActionResult Them([FromBody] Inventory newInven)
        {
            var existingInven = dbc.Inventories.FirstOrDefault(m => m.ItemId == newInven.ItemId);
            if (existingInven != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Item đã tồn tại." });
            }
            dbc.Inventories.Add(newInven);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Inventories.ToList() });
        }



        [HttpPut]
        [Route("/Inventory/Update")]
        public IActionResult Sua([FromBody] Inventory updateInven)
        {
            var existingInven = dbc.Inventories.FirstOrDefault(m => m.ItemId == updateInven.ItemId);
            if (existingInven == null)
            {
                return BadRequest(new { message = "Voucher không tồn tại." });
            }

            existingInven.ItemName = updateInven.ItemName;
            existingInven.Quantity = updateInven.Quantity;
            existingInven.SupplierId = updateInven.SupplierId;

            dbc.SaveChanges();

            return Ok(new { data = dbc.Inventories.ToList() });
        }
        [HttpGet]
        [Route("/Inventory/GetById")]
        public IActionResult GetById(int id)
        {
            var vc = dbc.Inventories.FirstOrDefault(m => m.ItemId == id);
            if (vc == null)
            {
                return NotFound(new { message = "Không tìm thấy với InventoriesId này." });
            }

            return Ok(vc);
        }
    }
}
