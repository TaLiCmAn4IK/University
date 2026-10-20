using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchShop.Entities
{
    /// <summary>
    /// Сущность "Заказ" (Code First).
    ///
    /// Связи:
    ///   - many-to-one к UserEntity     (много заказов → один пользователь)
    ///   - one-to-many к OrderItemEntity (один заказ → много позиций)
    /// </summary>
    [Table("Orders")]
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Новый";

        public ICollection<OrderItemEntity> Items { get; set; } = new List<OrderItemEntity>();
    }
}
