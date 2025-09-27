using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class HomestayRoomType
{
    public long RoomTypeId { get; set; }

    public long HomestayId { get; set; }

    public string Name { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public int TotalRooms { get; set; }

    public int? Capacity { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual Homestay Homestay { get; set; } = null!;
}
