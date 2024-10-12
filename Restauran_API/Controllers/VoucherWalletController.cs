using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class VoucherWalletController : ControllerBase
    {
        QLNhaHangContext dbc;
        public VoucherWalletController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/VoucherWallet/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.VoucherWallets.ToList());
        }
        [HttpPost]
        [Route("/VoucherWallet/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.VoucherWallets.FirstOrDefault(v => v.VoucherWalletId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.VoucherWallets.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.VoucherWallets.ToList());
        }
        [HttpPost]
        [Route("/VoucherWallet/Insert")]
        public IActionResult Them(int voucherid, int customerid, int quantity)
        {
            VoucherWallet hh = new VoucherWallet
            {
                VoucherId= voucherid,
                CustomerId = customerid,
                Quantity = quantity

            };

            dbc.VoucherWallets.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.VoucherWallets.ToList() });
        }



        [HttpPut]
        [Route("/VoucherWallet/Update")]
        public IActionResult Sua(int VoucherWalletId, int voucherid, int customerid, int quantity)
        {
            var hh = dbc.VoucherWallets.FirstOrDefault(c => c.VoucherWalletId == VoucherWalletId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với VoucherWalletId này." });
            }
                hh.VoucherId = voucherid;
                hh.CustomerId = customerid;
                hh.Quantity = quantity;
            dbc.SaveChanges();

            return Ok(new { data = dbc.VoucherWallets.ToList() });
        }
    }
}
