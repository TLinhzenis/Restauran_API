using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restauran_API.Models
{
    public partial class OrderItem
    {
        [Key]
        [Column("OrderItemID")]
        public int OrderItemId { get; set; }
        [Column("OrderID")]
        public int? OrderId { get; set; }
        [Column("MenuItemID")]
        public int? MenuItemId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [StringLength(500)]
        public string? Note { get; set; }
        [StringLength(100)]
        public string? Status { get; set; }

        [ForeignKey(nameof(MenuItemId))]
        [InverseProperty("OrderItems")]
        public virtual MenuItem? MenuItem { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderItems")]
        public virtual Order? Order { get; set; }
    }
}
