using System;
using System.Collections.Generic;

namespace WatchShop.GeneratedModels;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public int? FoundedYear { get; set; }

    public virtual ICollection<Watch> Watches { get; set; } = new List<Watch>();
}
