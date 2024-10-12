using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        QLNhaHangContext dbc;
        public SupplierController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Supplier/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Suppliers.ToList());
        }
        [HttpPost]
        [Route("/Supplier/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.Suppliers.FirstOrDefault(v => v.SupplierId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.Suppliers.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.Suppliers.ToList());
        }
        [HttpPost]
        [Route("/Supplier/Insert")]
        public IActionResult Them(string supplierName, string ContactInfo)
        {
            Supplier hh = new Supplier
            {
                SupplierName = supplierName,
                ContactInfo = ContactInfo

            };

            dbc.Suppliers.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Suppliers.ToList() });
        }



        [HttpPut]
        [Route("/Supplier/Update")]
        public IActionResult Sua(int SupplierId, string supplierName, string ContactInfo)
        {
            var hh = dbc.Suppliers.FirstOrDefault(c => c.SupplierId == SupplierId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với SupplierId này." });
            }
                hh.SupplierName = supplierName;
                hh.ContactInfo = ContactInfo;
            dbc.SaveChanges();

            return Ok(new { data = dbc.Suppliers.ToList() });
        }
    }
}
