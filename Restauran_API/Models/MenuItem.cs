using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class MenuItem
    {
        public MenuItem()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        [Column("MenuItemID")]
        public int MenuItemId { get; set; }
        [StringLength(200)]
        public string ItemName { get; set; } = null!;
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [StringLength(50)]
        public string? Category { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public string? Image { get; set; }

        [InverseProperty(nameof(OrderItem.MenuItem))]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
