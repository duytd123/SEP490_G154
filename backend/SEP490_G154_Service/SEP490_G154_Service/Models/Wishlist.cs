using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Wishlist
{
    public long WishlistId { get; set; }

    public long UserId { get; set; }

    public long? ProductId { get; set; }

    public long? HomestayId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Homestay? Homestay { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User User { get; set; } = null!;
}
