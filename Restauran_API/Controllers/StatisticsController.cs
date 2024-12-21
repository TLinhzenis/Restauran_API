using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restauran_API.Models;

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
        [Route("/Statistics/Revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var today = DateTime.Today;

            // Tính tổng doanh thu cho 7 ngày gần nhất
            var sevenDaysAgo = today.AddDays(-6);
            var revenueLast7Days = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date >= sevenDaysAgo && order.OrderTime.Value.Date <= today)
                .SumAsync(order => order.TotalAmount ?? 0);

            // Tính tổng doanh thu cho 30 ngày gần nhất
            var thirtyDaysAgo = today.AddDays(-29);
            var revenueLast30Days = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date >= thirtyDaysAgo && order.OrderTime.Value.Date <= today)
                .SumAsync(order => order.TotalAmount ?? 0);

            return Ok(new
            {
                Today = new { Date = today, TotalRevenue = revenueLast7Days },
                Last7Days = new { StartDate = sevenDaysAgo, EndDate = today, TotalRevenue = revenueLast7Days },
                Last30Days = new { StartDate = thirtyDaysAgo, EndDate = today, TotalRevenue = revenueLast30Days }
            });
        }

        [HttpGet]
        [Route("/Statistics/OrderCount")]
        public async Task<IActionResult> GetOrderCount()
        {
            var today = DateTime.Today;

            // Đếm số lượng đơn hàng cho ngày hôm nay
            var todayOrderCount = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date == today)
                .CountAsync();

            // Đếm số lượng đơn hàng cho 7 ngày gần nhất
            var sevenDaysAgo = today.AddDays(-6);
            var last7DaysOrderCount = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date >= sevenDaysAgo && order.OrderTime.Value.Date <= today)
                .CountAsync();

            // Đếm số lượng đơn hàng cho 30 ngày gần nhất
            var thirtyDaysAgo = today.AddDays(-29);
            var last30DaysOrderCount = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date >= thirtyDaysAgo && order.OrderTime.Value.Date <= today)
                .CountAsync();

            return Ok(new
            {
                Today = new { Date = today, OrderCount = todayOrderCount },
                Last7Days = new { StartDate = sevenDaysAgo, EndDate = today, OrderCount = last7DaysOrderCount },
                Last30Days = new { StartDate = thirtyDaysAgo, EndDate = today, OrderCount = last30DaysOrderCount }
            });
        }

        [HttpGet]
        [Route("/Statistics/Top3Customer")]
        public async Task<IActionResult> GetTop3Customer()
        {
            var topCustomers = await dbc.Customers
            .Select(c => new
            {
                FullName = c.FullName,
                TotalSpent = c.Orders.Sum(o => o.TotalAmount),
                TotalOrders = c.Orders.Count()
            })
            .OrderByDescending(c => c.TotalSpent)
            .Take(3)
            .ToListAsync();

            return Ok(topCustomers);
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