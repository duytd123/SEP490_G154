using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class CartItem
{
    public long CartItemId { get; set; }

    public long CartId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
