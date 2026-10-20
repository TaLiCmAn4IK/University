using System;
using System.Collections.Generic;

namespace WatchShop.GeneratedModels;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int WatchId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Watch Watch { get; set; } = null!;
}
