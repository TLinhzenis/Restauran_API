using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        QLNhaHangContext dbc;
        public VoucherController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Voucher/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Vouchers.ToList());
        }
        [HttpPost]
        [Route("/Voucher/Delete")]
        public IActionResult Xoa(int id)
        {
            // Tải voucher cùng với các VoucherWallets liên quan
            var voucher = dbc.Vouchers
                             .Include(v => v.VoucherWallets) // Bao gồm các VoucherWallets liên quan
                             .FirstOrDefault(v => v.VoucherId == id);

            if (voucher == null)
            {
                return NotFound();
            }

            // Xóa tất cả các bản ghi VoucherWallets liên quan
            dbc.VoucherWallets.RemoveRange(voucher.VoucherWallets);

            // Cuối cùng, xóa voucher
            dbc.Vouchers.Remove(voucher);

            dbc.SaveChanges();

            return Ok(dbc.Vouchers.ToList());
        }

        [HttpPost]
        [Route("/Voucher/Insert")]
        public IActionResult Them([FromBody] Voucher newVoucher)
        {
            var existingVoucher = dbc.Vouchers.FirstOrDefault(m => m.VoucherId == newVoucher.VoucherId);
            if (existingVoucher != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Voucher đã tồn tại." });
            }
            dbc.Vouchers.Add(newVoucher);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Vouchers.ToList() });
        }



        [HttpPut]
        [Route("/Voucher/Update")]
        public IActionResult Sua([FromBody] Voucher updateVoucher)
        {
            var existingVoucher = dbc.Vouchers.FirstOrDefault(m => m.VoucherId == updateVoucher.VoucherId);
            if (existingVoucher == null)
            {
                return BadRequest(new { message = "Voucher không tồn tại." });
            }

            existingVoucher.VoucherType = updateVoucher.VoucherType;
            existingVoucher.VoucherPoint = updateVoucher.VoucherPoint;

            dbc.SaveChanges();

            return Ok(new { data = dbc.Vouchers.ToList() });
        }

        [HttpGet]
        [Route("/Voucher/GetById")]
        public IActionResult GetById(int id)
        {
            var vc = dbc.Vouchers.FirstOrDefault(m => m.VoucherId == id);
            if (vc == null)
            {
                return NotFound(new { message = "Không tìm thấy với VoucherId này." });
            }

            return Ok(vc);
        }
        [HttpPost]
        [Route("/Voucher/Buy")]
        public IActionResult MuaVoucher(int customerId, int voucherId, int quantity)
        {
            var customer = dbc.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy khách hàng với ID này." });
            }

            var voucher = dbc.Vouchers.FirstOrDefault(v => v.VoucherId == voucherId);
            if (voucher == null)
            {
                return NotFound(new { message = "Không tìm thấy voucher với ID này." });
            }

            int totalPointsRequired = (voucher.VoucherPoint ?? 0) * quantity;

            if (customer.Point < totalPointsRequired)
            {
                return BadRequest(new { message = "Điểm không đủ để mua voucher." });
            }

            customer.Point -= totalPointsRequired;

            var existingVoucherWallet = dbc.VoucherWallets
                .FirstOrDefault(vw => vw.CustomerId == customerId && vw.VoucherId == voucherId);

            if (existingVoucherWallet != null)
            {
                existingVoucherWallet.Quantity += quantity;
            }
            else
            {
                var newVoucherWallet = new VoucherWallet
                {
                    CustomerId = customerId,
                    VoucherId = voucherId,
                    Quantity = quantity
                };
                dbc.VoucherWallets.Add(newVoucherWallet);
            }

            dbc.SaveChanges();

            return Ok(new
            {
                message = "Mua voucher thành công.",
                customerPoints = customer.Point,
                voucherWallet = dbc.VoucherWallets
                    .Where(vw => vw.CustomerId == customerId)
                    .ToList()
            });
        }

    }
}
