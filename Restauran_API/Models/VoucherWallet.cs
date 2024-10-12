using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    [Table("VoucherWallet")]
    public partial class VoucherWallet
    {
        [Key]
        [Column("VoucherWalletID")]
        public int VoucherWalletId { get; set; }
        [Column("VoucherID")]
        public int? VoucherId { get; set; }
        [Column("CustomerID")]
        public int? CustomerId { get; set; }
        public int? Quantity { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("VoucherWallets")]
        public virtual Customer? Customer { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("VoucherWallets")]
        public virtual Voucher? Voucher { get; set; }
    }
}
