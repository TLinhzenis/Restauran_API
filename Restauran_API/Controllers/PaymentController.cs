using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        QLNhaHangContext dbc;
        public PaymentController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Payment/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Payments.ToList());
        }
        [HttpPost]
        [Route("/Payment/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.Payments.FirstOrDefault(v => v.PaymentId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.Payments.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.Payments.ToList());
        }
        [HttpPost]
        [Route("/Payment/Insert")]
        public IActionResult Them(int orderid, string paymentmethod, decimal AmountPaid, DateTime PaymentTime)
        {
            Payment hh = new Payment
            {
                OrderId = orderid,
                PaymentMethod = paymentmethod,
                AmountPaid = AmountPaid,
                PaymentTime = PaymentTime

            };

            dbc.Payments.Add(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Payments.ToList() });
        }



        [HttpPut]
        [Route("/Payment/Update")]
        public IActionResult Sua(int PaymentId, int orderid, string paymentmethod, decimal AmountPaid, DateTime PaymentTime)
        {
            var hh = dbc.Payments.FirstOrDefault(c => c.PaymentId == PaymentId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với PaymentId này." });
            }
                hh.OrderId = orderid;
                hh.PaymentMethod = paymentmethod;
                hh.AmountPaid = AmountPaid;
                hh.PaymentTime = PaymentTime;
            dbc.SaveChanges();

            return Ok(new { data = dbc.Payments.ToList() });
        }
    }
}
