using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            Payments = new HashSet<Payment>();
        }

        [Key]
        [Column("OrderID")]
        public int OrderId { get; set; }
        [Column("TableID")]
        public int? TableId { get; set; }
        [Column("CustomerID")]
        public int? CustomerId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrderTime { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalAmount { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Orders")]
        public virtual Customer? Customer { get; set; }
        [ForeignKey(nameof(TableId))]
        [InverseProperty("Orders")]
        public virtual Table? Table { get; set; }
        [InverseProperty(nameof(OrderItem.Order))]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [InverseProperty(nameof(Payment.Order))]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
