using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class BookingItem
{
    public long BookingItemId { get; set; }

    public long BookingId { get; set; }

    public long RoomTypeId { get; set; }

    public int RoomsBooked { get; set; }

    public decimal PricePerNight { get; set; }

    public int Nights { get; set; }

    public decimal LineTotal { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual HomestayRoomType RoomType { get; set; } = null!;
}
