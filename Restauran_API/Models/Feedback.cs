using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    [Table("Feedback")]
    public partial class Feedback
    {
        [Key]
        [Column("FeedbackID")]
        public int FeedbackId { get; set; }
        [Column("CustomerID")]
        public int? CustomerId { get; set; }
        public int? Rating { get; set; }
        [StringLength(500)]
        public string? Comment { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubmittedAt { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Feedbacks")]
        public virtual Customer? Customer { get; set; }
    }
}
