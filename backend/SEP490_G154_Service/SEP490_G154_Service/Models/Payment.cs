using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Payment
{
    public long PaymentId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public string? Instructions { get; set; }

    public bool IsActive { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
