using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Booking
{
    public long BookingId { get; set; }

    public long CustomerId { get; set; }

    public long HomestayId { get; set; }

    public int? Status { get; set; }

    public DateOnly CheckinDate { get; set; }

    public DateOnly CheckoutDate { get; set; }

    public int Guests { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? VoucherCode { get; set; }

    public decimal? DiscountAmount { get; set; }

    public long? BookingVoucherId { get; set; }

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual BookingVoucher? BookingVoucher { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Homestay Homestay { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
