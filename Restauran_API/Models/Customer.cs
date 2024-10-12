using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Feedbacks = new HashSet<Feedback>();
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
            VoucherWallets = new HashSet<VoucherWallet>();
        }

        [Key]
        [Column("CustomerID")]
        public int CustomerId { get; set; }
        [StringLength(100)]
        public string Username { get; set; } = null!;
        [StringLength(100)]
        public string Password { get; set; } = null!;
        [StringLength(200)]
        public string? FullName { get; set; }
        [StringLength(100)]
        public string? Email { get; set; }
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateJoined { get; set; }
        public int? Point { get; set; }

        [InverseProperty(nameof(Feedback.Customer))]
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        [InverseProperty(nameof(Order.Customer))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Reservation.Customer))]
        public virtual ICollection<Reservation> Reservations { get; set; }
        [InverseProperty(nameof(VoucherWallet.Customer))]
        public virtual ICollection<VoucherWallet> VoucherWallets { get; set; }
    }
}
