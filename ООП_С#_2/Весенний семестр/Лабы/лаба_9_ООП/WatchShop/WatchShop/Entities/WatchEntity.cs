using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WatchShop.Entities
{
    /// <summary>
    /// Сущность "Товар/Часы" (Code First).
    ///
    /// Связи:
    ///   - many-to-one к ManufacturerEntity (много часов → один производитель)
    ///   - many-to-one к CategoryEntity     (много часов → одна категория)
    ///   - one-to-many к OrderItemEntity    (один товар → много позиций заказа)
    ///
    /// [ForeignKey] явно указывает, какое скалярное свойство является
    /// внешним ключом для навигационного свойства.
    /// </summary>
    [Table("Watches")]
    public class WatchEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShortName { get; set; } = "";

        [MaxLength(200)]
        public string? FullName { get; set; }

        public string? Description { get; set; }

        // ----- Внешний ключ + навигация к производителю -----
        public int? ManufacturerId { get; set; }

        [ForeignKey(nameof(ManufacturerId))]
        public ManufacturerEntity? Manufacturer { get; set; }

        // ----- Внешний ключ + навигация к категории -----
        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public CategoryEntity? Category { get; set; }

        // ----- Скалярные поля -----
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }

        public double Discount { get; set; }

        public bool InStock { get; set; } = true;

        [MaxLength(50)] public string? Color { get; set; }
        [MaxLength(20)] public string? Size { get; set; }
        [MaxLength(50)] public string? Material { get; set; }
        [MaxLength(20)] public string? Gender { get; set; }
        [MaxLength(50)] public string? MovementType { get; set; }
        [MaxLength(20)] public string? WaterResistance { get; set; }

        public int SoldCount { get; set; }

        /// <summary>BLOB-картинка (VARBINARY(MAX)).</summary>
        public byte[]? Image { get; set; }

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>Навигация: позиции заказов, в которых фигурирует этот товар.</summary>
        public ICollection<OrderItemEntity> OrderItems { get; set; } = new List<OrderItemEntity>();
    }
}
