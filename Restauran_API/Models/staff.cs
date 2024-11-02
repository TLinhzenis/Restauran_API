using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    [Table("Staff")]
    public partial class staff
    {
        public staff()
        {
            Shifts = new HashSet<Shift>();
        }

        [Key]
        [Column("StaffID")]
        public int StaffId { get; set; }
        [StringLength(100)]
        public string Username { get; set; } = null!;
        [StringLength(100)]
        public string Password { get; set; } = null!;
        [StringLength(200)]
        public string? FullName { get; set; }
        [StringLength(50)]
        public string Role { get; set; } = null!;
        public string? Image { get; set; }

        [InverseProperty(nameof(Shift.Staff))]
        public virtual ICollection<Shift> Shifts { get; set; }
    }
}
