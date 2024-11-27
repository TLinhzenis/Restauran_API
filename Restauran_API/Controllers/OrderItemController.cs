using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        QLNhaHangContext dbc;
        public OrderItemController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/OrderItem/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.OrderItems.ToList());
        }
        [HttpPost]
        [Route("/OrderItem/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.OrderItems.FirstOrDefault(v => v.OrderItemId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.OrderItems.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.OrderItems.ToList());
        }
        [HttpPost]
        [Route("/OrderItem/Insert")]
        public IActionResult Them([FromBody] OrderItem orderItem)
        {
            OrderItem hh = new OrderItem
            {
                OrderId = orderItem.OrderId,
                MenuItemId = orderItem.MenuItemId,
                Quantity = orderItem.Quantity,
                Price = (decimal)orderItem.Price,
                Note = orderItem.Note,
                Status = orderItem.Status,
            };

            dbc.OrderItems.Add(hh);
            dbc.SaveChanges();

            return Ok(hh);
        }



        [HttpPut]
        [Route("/OrderItem/Update")]
        public IActionResult Sua(int OrderItemId, string orderStatus)
        {
            var hh = dbc.OrderItems.FirstOrDefault(c => c.OrderItemId == OrderItemId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với OrderItemId này." });
            }
            hh.Status = orderStatus;
            dbc.Update(hh);
            dbc.SaveChanges();

            return Ok(new { data = dbc.OrderItems.ToList() });
        }
        [HttpGet]
        [Route("/OrderItem/ListByOrderId/{orderId}")]
        public IActionResult GetListByOrderId(int orderId)
        {
            var orderItems = dbc.OrderItems.Where(oi => oi.OrderId == orderId).ToList();

            if (orderItems == null || !orderItems.Any())
            {
                return NotFound(new { message = "Không tìm thấy OrderItem cho OrderId này." });
            }

            return Ok(orderItems);
        }

    }
}
