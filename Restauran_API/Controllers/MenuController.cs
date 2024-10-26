using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Restauran_API.DTO;
using Restauran_API.Models;
using Restauran_API.SignalR;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly QLNhaHangContext dbc;
        private readonly IHubContext<MenuItemHub> _hubContext;
        private readonly IHostEnvironment _hostEnvironment; // Thêm trường host environment

        public MenuController(QLNhaHangContext db, IHubContext<MenuItemHub> hubContext, IHostEnvironment hostEnvironment)
        {
            dbc = db;
            _hubContext = hubContext;
            _hostEnvironment = hostEnvironment; // Khởi tạo trường
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

            // Xóa hình ảnh liên quan
            if (!string.IsNullOrEmpty(menuItem.Image)) // Kiểm tra xem có hình ảnh không
            {
                DeleteImage(menuItem.Image);
            }

            dbc.OrderItems.RemoveRange(menuItem.OrderItems);
            dbc.MenuItems.Remove(menuItem);
            dbc.SaveChanges();

            return Ok(dbc.MenuItems.ToList());
        }

        private IActionResult DeleteImage(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return BadRequest("Tên hình ảnh không hợp lệ.");

            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/uploads");
            var filePath = Path.Combine(uploadsFolder, imageName);

            // Kiểm tra xem file có tồn tại không
            if (!System.IO.File.Exists(filePath))
                return NotFound("Hình ảnh không tồn tại.");

            // Xóa file hình ảnh
            System.IO.File.Delete(filePath);
            return Ok("Hình ảnh đã được xóa thành công.");
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
            existingMenuItem.Image = updatedMenuItem.Image;

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
        [HttpGet]
        [Route("/Menu/GetTop5OrderByQuantity")]
        public async Task<ActionResult<List<MenuItemDTO>>> GetTop5OrderByQuantity()
        {
            var result = await dbc.OrderItems
           .GroupBy(oi => new { oi.MenuItem.MenuItemId, oi.MenuItem.ItemName, oi.MenuItem.Image })
           .Select(group => new MenuItemDTO
           {
               MenuItemID = group.Key.MenuItemId,
               ItemName = group.Key.ItemName,
               TotalQuantity = group.Sum(oi => oi.Quantity),
               Image = group.Key.Image,
           })
           .OrderByDescending(dto => dto.TotalQuantity)
           .Take(5)
           .ToListAsync();

            return Ok(result);
        }
    }
}
