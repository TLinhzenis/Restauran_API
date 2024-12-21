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
        public class BuyVoucherRequest
        {
            public int CustomerId { get; set; }
            public int VoucherId { get; set; }
            public int VoucherPrice { get; set; }
        }
        [HttpPost]
        [Route("/Voucher/Buy")]
        public IActionResult BuyVoucher([FromBody] BuyVoucherRequest request)
        {
            // Lấy thông tin khách hàng từ database
            var customer = dbc.Customers.FirstOrDefault(c => c.CustomerId == request.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = "Khách hàng không tồn tại" });
            }

            // Kiểm tra đủ điểm không
            if (customer.Point < request.VoucherPrice)
            {
                return BadRequest(new { message = "Bạn không đủ điểm để mua voucher" });
            }

            // Trừ điểm của khách hàng
            customer.Point -= request.VoucherPrice;
            dbc.SaveChanges();

            // Kiểm tra voucher có trong database không
            var voucher = dbc.Vouchers.FirstOrDefault(v => v.VoucherId == request.VoucherId);
            if (voucher == null)
            {
                return BadRequest(new { message = "Voucher không tồn tại" });
            }

            // Kiểm tra xem voucher đó đã có trong VoucherWallet chưa
            var voucherWallet = dbc.VoucherWallets.FirstOrDefault(vw => vw.CustomerId == request.CustomerId && vw.VoucherId == request.VoucherId);
            if (voucherWallet != null)
            {
                // Nếu đã có voucher trong Wallet, tăng quantity
                voucherWallet.Quantity += 1;
            }
            else
            {
                // Nếu chưa có voucher trong Wallet, thêm mới vào
                var newVoucherWallet = new VoucherWallet
                {
                    CustomerId = request.CustomerId,
                    VoucherId = request.VoucherId,
                    Quantity = 1
                };
                dbc.VoucherWallets.Add(newVoucherWallet);
            }

            // Lưu lại các thay đổi
            dbc.SaveChanges();

            return Ok(new { message = "Mua voucher thành công", pointsRemaining = customer.Point });
        }

    }
}
