using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        QLNhaHangContext dbc;
        public OrderController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Order/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.Orders.ToList());
        }
        [HttpPost]
        [Route("/Order/Delete")]
        public IActionResult Xoa(int id)
        {
            var order = dbc.Orders
                           .Include(o => o.OrderItems)
                           .Include(o => o.Payments)
                           .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Xóa tất cả các bản ghi liên quan
            dbc.OrderItems.RemoveRange(order.OrderItems);
            dbc.Payments.RemoveRange(order.Payments);

            // Cuối cùng, xóa đơn hàng
            dbc.Orders.Remove(order);

            dbc.SaveChanges();

            return Ok(dbc.Orders.ToList());
        }

        [HttpPost]
        [Route("/Order/Insert")]
        public IActionResult Them([FromBody] Order orderDto)
        {
            Order hh = new Order
            {
                TableId = orderDto.TableId,
                OrderTime = orderDto.OrderTime,
                Status = orderDto.Status
            };
            dbc.Orders.Add(hh);
            dbc.SaveChanges();
            return Ok(hh);
        }
        
        [HttpPut]
        [Route("/Order/Update")]
        public IActionResult Sua([FromBody] Order o)
        {
            var hh = dbc.Orders.FirstOrDefault(c => c.OrderId == o.OrderId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với OrderId này." });
            }

            hh.TableId = o.TableId;
            hh.OrderTime = o.OrderTime;
            hh.TotalAmount = o.TotalAmount;
            hh.Status = o.Status;

            dbc.SaveChanges();

            return Ok(hh);  // Return the updated order
        }
        [HttpGet]
        [Route("/Order/GetById")]
        public IActionResult GetById(int tableID)
        {
            // Lấy danh sách các Order dựa trên TableId
            var orders = dbc.Orders.Where(m => m.TableId == tableID).ToList();

            // Kiểm tra nếu không có đơn hàng nào thỏa mãn
            if (orders == null || orders.Count == 0)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng nào với TableId này." });
            }

            // Trả về danh sách các Order
            return Ok(orders);
        }

    }
}
