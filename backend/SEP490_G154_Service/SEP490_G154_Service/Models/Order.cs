using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public long CustomerId { get; set; }

    public decimal TotalAmount { get; set; }

    public int? Status { get; set; }

    public string? ShippingAddress { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? VoucherCode { get; set; }

    public decimal? DiscountAmount { get; set; }

    public long? ProductVoucherId { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ProductVoucher? ProductVoucher { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
