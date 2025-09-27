using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Cart
{
    public long CartId { get; set; }

    public long CustomerId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User Customer { get; set; } = null!;
}
