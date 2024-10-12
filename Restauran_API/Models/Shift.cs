using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Shift
    {
        [Key]
        [Column("ShiftID")]
        public int ShiftId { get; set; }
        [Column("StaffID")]
        public int? StaffId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }

        [ForeignKey(nameof(StaffId))]
        [InverseProperty(nameof(staff.Shifts))]
        public virtual staff? Staff { get; set; }
    }
}
