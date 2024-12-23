using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var supplier = dbc.Suppliers.Include(s => s.Inventories).FirstOrDefault(s => s.SupplierId == id);
            if (supplier == null)
            {
                return NotFound(new { message = "Nhà cung cấp không tồn tại." });
            }

            if (supplier.Inventories.Any())
            {
                return BadRequest(new { message = "Mặt hàng từ nhà cung cấp này còn trong kho." });
            }

            dbc.Suppliers.Remove(supplier);
            dbc.SaveChanges();

            return Ok(new { message = "Nhà cung cấp đã được xóa thành công!" });
        }

        [HttpPost]
        [Route("/Supplier/Insert")]
        public IActionResult Them([FromBody] Supplier newSupplier)
        {
            var existingSupplier = dbc.Suppliers.FirstOrDefault(m => m.SupplierId == newSupplier.SupplierId);
            if (existingSupplier != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Suppliers đã tồn tại." });
            }
            dbc.Suppliers.Add(newSupplier);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Suppliers.ToList() });
        }



        [HttpPut]
        [Route("/Supplier/Update")]
        public IActionResult Sua([FromBody] Supplier updateSupplier)
        {
            var existingSupplier = dbc.Suppliers.FirstOrDefault(m => m.SupplierId == updateSupplier.SupplierId);
            if (existingSupplier == null)
            {
                return BadRequest(new { message = "Voucher không tồn tại." });
            }

            existingSupplier.SupplierName = updateSupplier.SupplierName;
            existingSupplier.ContactInfo = updateSupplier.ContactInfo;

            dbc.SaveChanges();

            return Ok(new { data = dbc.Suppliers.ToList() });
        }
        [HttpGet]
        [Route("/Supplier/GetById")]
        public IActionResult GetById(int id)
        {
            var vc = dbc.Suppliers.FirstOrDefault(m => m.SupplierId == id);
            if (vc == null)
            {
                return NotFound(new { message = "Không tìm thấy với SupplierId này." });
            }

            return Ok(vc);
        }
    }
}
