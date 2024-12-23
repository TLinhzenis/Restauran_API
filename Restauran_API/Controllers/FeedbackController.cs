using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restauran_API.Models;

namespace Restauran_API.Controllers
{
    [Route("controller")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        QLNhaHangContext dbc;
        public FeedbackController(QLNhaHangContext db)
        {
            dbc = db;
        }
        [HttpGet]
        [Route("/Feedback/List")]
        public IActionResult GetList()
        {
            var topFeedbacks = dbc.Feedbacks
        .OrderByDescending(f => f.SubmittedAt)
        .Take(3)
        .ToList();

            return Ok(topFeedbacks);
        }
        [HttpGet]
        [Route("/Feedback/ListByRating")]
        public IActionResult GetListByRating(int rating)
        {
            var feedbacks = dbc.Feedbacks
                .Where(f => f.Rating == rating) // Lọc theo rating
                .OrderByDescending(f => f.SubmittedAt) // Sắp xếp theo ngày gửi
                .Take(3) // Lấy 3 feedback mới nhất
                .ToList();

            return Ok(feedbacks);
        }

        [HttpPost]
        [Route("/Feedback/Delete")]
        public IActionResult Xoa(int id)
        {
            var vc = dbc.Feedbacks.FirstOrDefault(v => v.FeedbackId == id);
            if (vc == null)
            {
                return NotFound();
            }
            dbc.Feedbacks.Remove(vc);
            dbc.SaveChanges();
            return Ok(dbc.Feedbacks.ToList());
        }
        [HttpPost]
        [Route("/Feedback/Insert")]
        public IActionResult Them([FromBody] Feedback newFeedback)
        {
            var existingFeedback = dbc.Feedbacks.FirstOrDefault(m => m.FeedbackId == newFeedback.FeedbackId);
            if (existingFeedback != null)
            {
                // Trả về mã lỗi và thông báo rằng món ăn đã tồn tại
                return BadRequest(new { message = "Feedbacks đã tồn tại." });
            }
            dbc.Feedbacks.Add(newFeedback);
            dbc.SaveChanges();

            return Ok(new { data = dbc.Feedbacks.ToList() });
        }



        [HttpPut]
        [Route("/Feedback/Update")]
        public IActionResult Sua(int FeedbackId, int customerid, int rating, string comment, DateTime SubmittedAt)
        {
            var hh = dbc.Feedbacks.FirstOrDefault(c => c.FeedbackId == FeedbackId);

            if (hh == null)
            {
                return NotFound(new { message = "Không tìm thấy với FeedbackId này." });
            }
            hh.CustomerId = customerid;
            hh.Rating = rating;
            hh.Comment = comment;
            hh.SubmittedAt = SubmittedAt;
            dbc.SaveChanges();

            return Ok(new { data = dbc.Feedbacks.ToList() });
        }
    }
}
