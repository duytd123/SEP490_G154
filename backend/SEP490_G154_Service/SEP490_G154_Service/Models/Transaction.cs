using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Transaction
{
    public long TransactionId { get; set; }

    public long? OrderId { get; set; }

    public long? BookingId { get; set; }

    public long PaymentId { get; set; }

    public string? OrderDescription { get; set; }

    public string? PaymentMethod { get; set; }

    public string? VnPayResponseCode { get; set; }

    public decimal Amount { get; set; }

    public DateTime? DateCreated { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
