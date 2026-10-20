using System;
using System.Collections.Generic;

namespace WatchShop.GeneratedModels;

public partial class Watch
{
    public int Id { get; set; }

    public string ShortName { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Description { get; set; }

    public int? ManufacturerId { get; set; }

    public int? CategoryId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public double Rating { get; set; }

    public double Discount { get; set; }

    public bool InStock { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public string? Material { get; set; }

    public string? Gender { get; set; }

    public string? MovementType { get; set; }

    public string? WaterResistance { get; set; }

    public int SoldCount { get; set; }

    public byte[]? Image { get; set; }

    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
