using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Table
    {
        public Table()
        {
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
        }

        [Key]
        [Column("TableID")]
        public int TableId { get; set; }
        [StringLength(10)]
        public string TableNumber { get; set; } = null!;
        public int Capacity { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        [StringLength(100)]
        public string Username { get; set; } = null!;
        [StringLength(100)]
        public string Password { get; set; } = null!;

        [InverseProperty(nameof(Order.Table))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Reservation.Table))]
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
