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
            return Ok(dbc.Feedbacks.ToList());
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
        public IActionResult Them(int customerid, int rating, string comment, DateTime SubmittedAt)
        {
            Feedback hh = new Feedback
            {
                CustomerId = customerid,
                Rating = rating,
                Comment = comment,
                SubmittedAt = SubmittedAt
                
            };

            dbc.Feedbacks.Add(hh);
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
