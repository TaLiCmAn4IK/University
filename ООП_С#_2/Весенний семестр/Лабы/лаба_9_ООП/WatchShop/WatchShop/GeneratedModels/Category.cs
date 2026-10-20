using System;
using System.Collections.Generic;

namespace WatchShop.GeneratedModels;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? NameKey { get; set; }

    public virtual ICollection<Watch> Watches { get; set; } = new List<Watch>();
}
