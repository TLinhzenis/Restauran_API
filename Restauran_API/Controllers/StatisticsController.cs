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
            var today = DateTime.Today.AddDays(1);
            var revenueToday = await dbc.Orders.Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date.Day == today.Day).SumAsync(order => order.TotalAmount ?? 0);
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
                Today = new { Date = today, TotalRevenue = revenueToday },
                Last7Days = new { StartDate = sevenDaysAgo, EndDate = today, TotalRevenue = revenueLast7Days },
                Last30Days = new { StartDate = thirtyDaysAgo, EndDate = today, TotalRevenue = revenueLast30Days }
            });
        }

        [HttpGet]
        [Route("/Statistics/OrderCount")]
        public async Task<IActionResult> GetOrderCount()
        {
            var today = DateTime.Today.AddDays(1);

            // Đếm số lượng đơn hàng cho ngày hôm nay
            var todayOrderCount = await dbc.Orders
                .Where(order => order.OrderTime.HasValue && order.OrderTime.Value.Date.Day == today.Day)
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
            var currentDate = DateTime.Now;
            var startDate = currentDate.AddMonths(-11); // Lấy từ tháng hiện tại ngược về 12 tháng

            // Lọc và nhóm dữ liệu
            var query = await dbc.Orders
                .Where(order => order.OrderTime.HasValue
                                && order.OrderTime.Value >= startDate
                                && order.OrderTime.Value <= currentDate) // Lọc trong khoảng thời gian
                .GroupBy(order => new
                {
                    Year = order.OrderTime.Value.Year,
                    Month = order.OrderTime.Value.Month
                }) // Nhóm theo năm/tháng
                .Select(group => new
                {
                    group.Key.Year,
                    group.Key.Month,
                    Revenue = group.Sum(order => order.TotalAmount ?? 0) // Tổng doanh thu
                })
                .ToListAsync();

            // Xử lý việc chuyển đổi chuỗi trên client
            var monthlyRevenue = query
                .AsEnumerable()
                .Select(result => new
                {
                    Month = $"{result.Year}-{result.Month:D2}", // Tạo chuỗi yyyy-MM
                    result.Revenue
                })
                .OrderBy(result => result.Month) // Sắp xếp theo yyyy-MM
                .ToList();

            return Ok(monthlyRevenue); // Trả về dữ liệu
        }



    }
}