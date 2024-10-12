using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Restauran_API.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            VoucherWallets = new HashSet<VoucherWallet>();
        }

        [Key]
        [Column("VoucherID")]
        public int VoucherId { get; set; }
        public int VoucherType { get; set; }
        public int? VoucherPoint { get; set; }

        [InverseProperty(nameof(VoucherWallet.Voucher))]
        public virtual ICollection<VoucherWallet> VoucherWallets { get; set; }
    }
}
