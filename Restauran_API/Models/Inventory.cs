using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    [Table("Inventory")]
    public partial class Inventory
    {
        [Key]
        [Column("ItemID")]
        public int ItemId { get; set; }
        [StringLength(200)]
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        [Column("SupplierID")]
        public int? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("Inventories")]
        public virtual Supplier? Supplier { get; set; }
    }
}
