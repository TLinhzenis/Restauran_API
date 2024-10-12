using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Payment
    {
        [Key]
        [Column("PaymentID")]
        public int PaymentId { get; set; }
        [Column("OrderID")]
        public int? OrderId { get; set; }
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? AmountPaid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PaymentTime { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("Payments")]
        public virtual Order? Order { get; set; }
    }
}
