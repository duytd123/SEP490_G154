using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class ProductImage
{
    public long ImageId { get; set; }

    public long ProductId { get; set; }

    public string Url { get; set; } = null!;

    public int? SortOrder { get; set; }

    public virtual Product Product { get; set; } = null!;
}
