using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;
using Restauran_API.SignalR;
using static System.Net.Mime.MediaTypeNames;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        QLNhaHangContext dbc;
        private readonly IHubContext<MenuItemHub> _hubContext;
        public MenuController(QLNhaHangContext db, IHubContext<MenuItemHub> hubContext)
        {
            dbc = db;
            _hubContext = hubContext;
        }
        [HttpGet]
        [Route("/Menu/List")]
        public IActionResult GetList()
        {
            return Ok(dbc.MenuItems.ToList());
        }
        [HttpPost]
        [Route("/Menu/Delete")]
        public IActionResult Xoa(int id)
        {
            var menuItem = dbc.MenuItems
                              .Include(m => m.OrderItems) 
                              .FirstOrDefault(m => m.MenuItemId == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            dbc.OrderItems.RemoveRange(menuItem.OrderItems);

            dbc.MenuItems.Remove(menuItem);

            dbc.SaveChanges();

            return Ok(dbc.MenuItems.ToList());
        }

        [HttpPost]
        [Route("/Menu/Insert")]
        public async Task<IActionResult> Them([FromBody] MenuItem newMenu)
        {
            // Kiểm tra xem món ăn đã tồn tại chưa
            var existingMenu = dbc.MenuItems.FirstOrDefault(m => m.MenuItemId == newMenu.MenuItemId);
            if (existingMenu != null)
            {
                return BadRequest(new { message = "Món ăn đã tồn tại." });
            }

            // Lưu thông tin món ăn vào database
            dbc.MenuItems.Add(newMenu);
            await dbc.SaveChangesAsync();

            // Gửi thông báo tới các client về cập nhật menu
            await _hubContext.Clients.All.SendAsync("ReceiveMenuUpdate", "Update Menu");

            return Ok(new { data = dbc.MenuItems.ToList() });
        }


        [HttpPut]
        [Route("/Menu/Update")]
        public IActionResult Sua([FromBody] MenuItem updatedMenuItem)
        {
            var existingMenuItem = dbc.MenuItems.FirstOrDefault(m => m.MenuItemId == updatedMenuItem.MenuItemId);

            if (existingMenuItem == null)
            {
                return NotFound(new { message = "Không tìm thấy với MenuItemId này." });
            }

            // Cập nhật thông tin menu item
            existingMenuItem.ItemName = updatedMenuItem.ItemName;
            existingMenuItem.Price = updatedMenuItem.Price;
            existingMenuItem.Category = updatedMenuItem.Category;
            existingMenuItem.Description = updatedMenuItem.Description;
            existingMenuItem.Image = updatedMenuItem.Image; // Giữ nguyên kiểu byte[]

            dbc.SaveChanges();

            return Ok(new { data = dbc.MenuItems.ToList() });
        }
        [HttpGet]
        [Route("/Menu/GetById")]
        public IActionResult GetById(int id)
        {
            var menuItem = dbc.MenuItems.FirstOrDefault(m => m.MenuItemId == id);
            if (menuItem == null)
            {
                return NotFound(new { message = "Không tìm thấy với MenuItemId này." });
            }

            return Ok(menuItem);
        }



    }
}
