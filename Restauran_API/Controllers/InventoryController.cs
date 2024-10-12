using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(dbc.Inventories.ToList());
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
        public IActionResult Them(string itemname, int quantity, int supplierid)
        {
            Inventory hh = new Inventory
            {
                ItemName = itemname,
                Quantity = quantity,
                SupplierId = supplierid

            };

            dbc.Inventories.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Inventories.ToList() });
        }



        [HttpPut]
        [Route("/Inventory/Update")]
        public IActionResult Sua(int ItemId, string itemname, int quantity, int supplierid)
        {
            var hh = dbc.Inventories.FirstOrDefault(c => c.ItemId == ItemId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với InventoryId này." });
            }
            hh.ItemName = itemname;
            hh.Quantity = quantity;
            hh.SupplierId = supplierid;
            dbc.SaveChanges();

            return Ok(new { data = dbc.Inventories.ToList() });
        }
    }
}
