using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class UserProductVoucher
{
    public long UserProductVoucherId { get; set; }

    public long UserId { get; set; }

    public string Code { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal Value { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
