using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        QLNhaHangContext dbc;
        public CustomerController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Customer/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Customers.ToList());
        }
        [HttpPost]
        [Route("/Customer/Delete")]
        public IActionResult Xoa(int id)
        {
            var customer = dbc.Customers
                              .Include(c => c.Feedbacks)
                              .Include(c => c.Orders)
                              .Include(c => c.Reservations)
                              .Include(c => c.VoucherWallets)
                              .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Xóa tất cả các bản ghi liên quan
            dbc.Feedbacks.RemoveRange(customer.Feedbacks);
            dbc.Orders.RemoveRange(customer.Orders);
            dbc.Reservations.RemoveRange(customer.Reservations);
            dbc.VoucherWallets.RemoveRange(customer.VoucherWallets);

            // Cuối cùng, xóa khách hàng
            dbc.Customers.Remove(customer);

            dbc.SaveChanges();

            return Ok(dbc.Customers.ToList());
        }


        [HttpPost]
        [Route("/Customer/Insert")]
        public IActionResult Them([FromBody] Customer newCustomer)
        {
            var existingCustomer = dbc.Customers.FirstOrDefault(m => m.CustomerId == newCustomer.CustomerId);
            if (existingCustomer != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Tài khoản đã tồn tại." });
            }

            dbc.Customers.Add(newCustomer);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Customers.ToList() });
        }



        [HttpPut]
        [Route("/Customer/Update")]
        public IActionResult Sua([FromBody] Customer updateCustomer)
        {
            var existingCustomer = dbc.Customers.FirstOrDefault(m => m.CustomerId == updateCustomer.CustomerId);
            if (existingCustomer == null)
            {
                return BadRequest(new { message = "Tài khoản không tồn tại." });
            }

            existingCustomer.Username = updateCustomer.Username;
            existingCustomer.FullName = updateCustomer.FullName;
            existingCustomer.Password = updateCustomer.Password;
            existingCustomer.Email = updateCustomer.Email;
            existingCustomer.PhoneNumber = updateCustomer.PhoneNumber;
            existingCustomer.Point = updateCustomer.Point;

            dbc.SaveChanges();

            return Ok(new { data = dbc.Customers.ToList() });
        }
        [HttpGet]
        [Route("/Customer/GetById")]
        public IActionResult GetById(int id)
        {
            var account = dbc.Customers.FirstOrDefault(m => m.CustomerId == id);
            if (account == null)
            {
                return NotFound(new { message = "Không tìm thấy với CustomerId này." });
            }

            return Ok(account);
        }

    }
}
