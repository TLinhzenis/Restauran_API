using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Inventories = new HashSet<Inventory>();
        }

        [Key]
        [Column("SupplierID")]
        public int SupplierId { get; set; }
        [StringLength(200)]
        public string? SupplierName { get; set; }
        [StringLength(500)]
        public string? ContactInfo { get; set; }

        [InverseProperty(nameof(Inventory.Supplier))]
        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}
