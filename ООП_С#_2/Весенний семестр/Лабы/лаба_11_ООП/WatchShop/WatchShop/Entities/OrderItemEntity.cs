using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchShop.Entities
{
    /// <summary>
    /// Сущность "Позиция заказа" (Code First).
    /// Это связующая таблица: реализует many-to-many между Orders и Watches
    /// (один заказ содержит много товаров, один товар встречается во многих
    /// заказах), но с дополнительными полями Quantity/UnitPrice — поэтому
    /// оформлена явной сущностью, а не неявной many-to-many.
    ///
    /// Связи:
    ///   - many-to-one к OrderEntity (много позиций → один заказ)
    ///   - many-to-one к WatchEntity (много позиций → один товар)
    /// </summary>
    [Table("OrderItems")]
    public class OrderItemEntity
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderEntity? Order { get; set; }

        public int WatchId { get; set; }

        [ForeignKey(nameof(WatchId))]
        public WatchEntity? Watch { get; set; }

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
