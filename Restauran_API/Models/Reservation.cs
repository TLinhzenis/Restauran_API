using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Reservation
    {
        [Key]
        [Column("ReservationID")]
        public int ReservationId { get; set; }
        [Column("CustomerID")]
        public int? CustomerId { get; set; }
        [Column("TableID")]
        public int? TableId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReservationTime { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Reservations")]
        public virtual Customer? Customer { get; set; }
        [ForeignKey(nameof(TableId))]
        [InverseProperty("Reservations")]
        public virtual Table? Table { get; set; }
    }
}
