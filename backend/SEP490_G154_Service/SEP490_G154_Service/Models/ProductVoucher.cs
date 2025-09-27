using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class ProductVoucher
{
    public long ProductVoucherId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string DiscountType { get; set; } = null!;

    public decimal Value { get; set; }

    public int MaxUsage { get; set; }

    public int UsedCount { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
