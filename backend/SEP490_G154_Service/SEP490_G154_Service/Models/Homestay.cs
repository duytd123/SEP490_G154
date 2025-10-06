using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Homestay
{
    public long HomestayId { get; set; }

    public long HostId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? LocationText { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<HomesStayImage> HomesStayImages { get; set; } = new List<HomesStayImage>();

    public virtual ICollection<HomestayRoomType> HomestayRoomTypes { get; set; } = new List<HomestayRoomType>();

    public virtual User Host { get; set; } = null!;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
