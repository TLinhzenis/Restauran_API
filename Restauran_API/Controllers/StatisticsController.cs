using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Restauran_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly QLNhaHangContext dbc;

        public StatisticsController(QLNhaHangContext db)
        {
            dbc = db;
        }


        [HttpGet]
        [Route("/Statistics/TodayRevenue")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            var today = DateTime.Today;

            // Tính tổng doanh thu cho ngày hôm nay từ bảng Orders
            var totalRevenue = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date == today)
                .SumAsync(order => order.TotalAmount ?? 0);

            return Ok(new { Date = today, TotalRevenue = totalRevenue });
        }

        [HttpGet]
        [Route("/Statistics/YearlyRevenue")]
        public async Task<IActionResult> GetYearlyRevenue()
        {
            var currentYear = DateTime.Now.Year; // Lấy năm hiện tại

            // Lọc các đơn hàng trong năm hiện tại và nhóm theo tháng
            var monthlyRevenue = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Year == currentYear) // Lọc theo năm
                .GroupBy(order => order.OrderTime.Value.Month) // Nhóm theo tháng
                .Select(group => new
                {
                    Month = group.Key, // Tháng
                    Revenue = group.Sum(order => order.TotalAmount ?? 0) // Tổng doanh thu trong tháng
                })
                .OrderBy(result => result.Month) // Sắp xếp theo tháng
                .ToListAsync();

            return Ok(monthlyRevenue); // Trả về doanh thu theo tháng
        }
    }
}